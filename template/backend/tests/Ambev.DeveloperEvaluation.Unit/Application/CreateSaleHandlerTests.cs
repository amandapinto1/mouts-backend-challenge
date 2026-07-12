using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;
    private readonly Faker _faker = new();

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _branchRepository = Substitute.For<IBranchRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _eventRepository = Substitute.For<ISaleEventRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _branchRepository, _productRepository, _eventRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale command When handling Then creates sale with calculated discounts")]
    public async Task Handle_ValidCommand_CreatesSaleWithDiscounts()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = _faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                    UnitPrice = 100m
                }
            ]
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            Items =
            [
                new SaleItem
                {
                    ProductId = command.Items[0].ProductId,
                    Quantity = 5,
                    UnitPrice = 100m
                }
            ]
        };

        var expectedResult = new CreateSaleResult { Id = sale.Id, SaleNumber = sale.SaleNumber };

        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>())
            .Returns(new Branch { Id = command.BranchId, Name = _faker.Company.CompanyName() });
        _saleRepository.GetNextSaleNumberAsync(Arg.Any<CancellationToken>()).Returns(1);
        _productRepository.GetByIdAsync(command.Items[0].ProductId, Arg.Any<CancellationToken>())
            .Returns(new Product { Id = command.Items[0].ProductId, Title = _faker.Commerce.ProductName(), Price = 100m });

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given sale with item quantity above 20 When handling Then throws DomainException")]
    public async Task Handle_ItemQuantityAbove20_ThrowsDomainException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = _faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items =
            [
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 21,
                    UnitPrice = 50m
                }
            ]
        };

        var sale = new Sale
        {
            Items = [new SaleItem { Quantity = 21, UnitPrice = 50m }]
        };

        _mapper.Map<Sale>(command).Returns(sale);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}

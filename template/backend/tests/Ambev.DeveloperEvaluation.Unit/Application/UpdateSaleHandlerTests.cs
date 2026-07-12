using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
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

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;
    private readonly Faker _faker = new();

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _branchRepository = Substitute.For<IBranchRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _eventRepository = Substitute.For<ISaleEventRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_saleRepository, _branchRepository, _productRepository, _eventRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid update command When handling Then updates sale successfully")]
    public async Task Handle_ValidCommand_UpdatesSale()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var existingSale = new Sale
        {
            Id = saleId,
            SaleNumber = "SALE-000001",
            SaleDate = DateTime.UtcNow.AddDays(-1),
            CustomerId = Guid.NewGuid(),
            BranchId = branchId,
            Items = [new SaleItem { ProductId = productId, Quantity = 2, UnitPrice = 50m, TotalAmount = 100m }]
        };

        var command = new UpdateSaleCommand
        {
            Id = saleId,
            SaleNumber = "SALE-000001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            BranchId = branchId,
            Items = [new UpdateSaleItemCommand { ProductId = productId, Quantity = 5, UnitPrice = 100m }]
        };

        var expectedResult = new UpdateSaleResult { Id = saleId, SaleNumber = "SALE-000001" };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _branchRepository.GetByIdAsync(branchId, Arg.Any<CancellationToken>()).Returns(new Branch { Id = branchId, Name = "Branch A" });
        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(new Product { Id = productId, Title = "Product A", Price = 100m });
        _mapper.Map<List<SaleItem>>(command.Items).Returns([new SaleItem { ProductId = productId, Quantity = 5, UnitPrice = 100m }]);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(existingSale).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _eventRepository.Received(1).PublishEventAsync(Arg.Any<SaleEventDocument>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When updating Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = [new UpdateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given non-existent branch When updating Then throws KeyNotFoundException")]
    public async Task Handle_BranchNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            Id = saleId,
            BranchId = Guid.NewGuid(),
            Items = [new UpdateSaleItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(new Sale { Id = saleId });
        _branchRepository.GetByIdAsync(command.BranchId, Arg.Any<CancellationToken>()).Returns((Branch?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

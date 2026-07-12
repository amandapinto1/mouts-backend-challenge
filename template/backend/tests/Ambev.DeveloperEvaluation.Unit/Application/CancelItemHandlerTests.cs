using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CancelItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly ILogger<CancelItemHandler> _logger;
    private readonly CancelItemHandler _handler;
    private readonly Faker _faker = new();

    public CancelItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventRepository = Substitute.For<ISaleEventRepository>();
        _logger = Substitute.For<ILogger<CancelItemHandler>>();
        _handler = new CancelItemHandler(_saleRepository, _eventRepository, _logger);
    }

    [Fact(DisplayName = "Given valid sale and item When cancelling item Then item is cancelled and total recalculated")]
    public async Task Handle_ValidItem_CancelsAndRecalculates()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var saleId = Guid.NewGuid();

        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = _faker.Random.AlphaNumeric(10),
            Items =
            [
                new SaleItem { Id = itemId, ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 100m, TotalAmount = 450m, IsCancelled = false },
                new SaleItem { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 50m, TotalAmount = 100m, IsCancelled = false }
            ]
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        var command = new CancelItemCommand { SaleId = saleId, ItemId = itemId };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Contain("cancelled");
        sale.Items.First(i => i.Id == itemId).IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(100m);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _saleRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Sale?)null);
        var command = new CancelItemCommand { SaleId = Guid.NewGuid(), ItemId = Guid.NewGuid() };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given non-existent item When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_ItemNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = "S001",
            Items = [new SaleItem { Id = Guid.NewGuid(), ProductId = Guid.NewGuid() }]
        };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        var command = new CancelItemCommand { SaleId = sale.Id, ItemId = Guid.NewGuid() };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

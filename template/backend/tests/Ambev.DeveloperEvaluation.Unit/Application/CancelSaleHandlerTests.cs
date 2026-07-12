using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventRepository _eventRepository;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly CancelSaleHandler _handler;
    private readonly Faker _faker = new();

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventRepository = Substitute.For<ISaleEventRepository>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _handler = new CancelSaleHandler(_saleRepository, _eventRepository, _logger);
    }

    [Fact(DisplayName = "Given valid sale When cancelling Then sale and all items are cancelled")]
    public async Task Handle_ValidSale_CancelsSaleAndItems()
    {
        // Arrange
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = _faker.Random.AlphaNumeric(10),
            IsCancelled = false,
            Items =
            [
                new SaleItem { Id = Guid.NewGuid(), IsCancelled = false },
                new SaleItem { Id = Guid.NewGuid(), IsCancelled = false }
            ]
        };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        var command = new CancelSaleCommand { Id = sale.Id };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Contain("cancelled");
        sale.IsCancelled.Should().BeTrue();
        sale.Items.Should().AllSatisfy(i => i.IsCancelled.Should().BeTrue());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When cancelling Then throws KeyNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _saleRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Sale?)null);
        var command = new CancelSaleCommand { Id = Guid.NewGuid() };

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

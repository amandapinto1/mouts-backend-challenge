using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    private readonly Faker _faker = new();

    [Fact(DisplayName = "Given sale with items When calculating total Then sums non-cancelled items")]
    public void CalculateTotalAmount_WithItems_SumsNonCancelledItems()
    {
        // Arrange
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = _faker.Random.AlphaNumeric(10),
            Items =
            [
                new SaleItem { TotalAmount = 100m, IsCancelled = false },
                new SaleItem { TotalAmount = 200m, IsCancelled = false },
                new SaleItem { TotalAmount = 50m, IsCancelled = true }
            ]
        };

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(300m);
    }

    [Fact(DisplayName = "Given sale with all items cancelled When calculating total Then total is zero")]
    public void CalculateTotalAmount_AllCancelled_TotalIsZero()
    {
        // Arrange
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            Items =
            [
                new SaleItem { TotalAmount = 100m, IsCancelled = true },
                new SaleItem { TotalAmount = 200m, IsCancelled = true }
            ]
        };

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(0m);
    }

    [Fact(DisplayName = "Given sale with no items When calculating total Then total is zero")]
    public void CalculateTotalAmount_NoItems_TotalIsZero()
    {
        // Arrange
        var sale = new Sale { Id = Guid.NewGuid(), Items = [] };

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(0m);
    }
}

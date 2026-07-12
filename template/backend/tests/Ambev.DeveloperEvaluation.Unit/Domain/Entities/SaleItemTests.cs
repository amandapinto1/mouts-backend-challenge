using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    private readonly Faker _faker = new();

    private SaleItem CreateSaleItem(int quantity, decimal unitPrice = 100m) => new()
    {
        Id = Guid.NewGuid(),
        SaleId = Guid.NewGuid(),
        ProductId = Guid.NewGuid(),
        ProductName = _faker.Commerce.ProductName(),
        Quantity = quantity,
        UnitPrice = unitPrice
    };

    [Fact(DisplayName = "Given quantity below 4 When calculating discount Then no discount applied")]
    public void CalculateDiscount_QuantityBelow4_NoDiscount()
    {
        // Arrange
        var item = CreateSaleItem(quantity: 3, unitPrice: 100m);

        // Act
        item.CalculateDiscount();

        // Assert
        item.Discount.Should().Be(0m);
        item.TotalAmount.Should().Be(300m);
    }

    [Theory(DisplayName = "Given quantity between 4 and 9 When calculating discount Then 10% discount applied")]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(9)]
    public void CalculateDiscount_QuantityBetween4And9_10PercentDiscount(int quantity)
    {
        // Arrange
        var item = CreateSaleItem(quantity: quantity, unitPrice: 100m);

        // Act
        item.CalculateDiscount();

        // Assert
        item.Discount.Should().Be(0.10m);
        item.TotalAmount.Should().Be(quantity * 100m * 0.90m);
    }

    [Theory(DisplayName = "Given quantity between 10 and 20 When calculating discount Then 20% discount applied")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void CalculateDiscount_QuantityBetween10And20_20PercentDiscount(int quantity)
    {
        // Arrange
        var item = CreateSaleItem(quantity: quantity, unitPrice: 100m);

        // Act
        item.CalculateDiscount();

        // Assert
        item.Discount.Should().Be(0.20m);
        item.TotalAmount.Should().Be(quantity * 100m * 0.80m);
    }

    [Fact(DisplayName = "Given quantity above 20 When calculating discount Then throws DomainException")]
    public void CalculateDiscount_QuantityAbove20_ThrowsDomainException()
    {
        // Arrange
        var item = CreateSaleItem(quantity: 21);

        // Act
        var act = () => item.CalculateDiscount();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*20*");
    }

    [Fact(DisplayName = "Given quantity of 1 When calculating discount Then discount is zero")]
    public void CalculateDiscount_SingleItem_ZeroDiscount()
    {
        // Arrange
        var item = CreateSaleItem(quantity: 1, unitPrice: 50m);

        // Act
        item.CalculateDiscount();

        // Assert
        item.Discount.Should().Be(0m);
        item.TotalAmount.Should().Be(50m);
    }

    [Fact(DisplayName = "Given quantity of exactly 4 When calculating discount Then 10% discount applied")]
    public void CalculateDiscount_ExactlyFour_10PercentDiscount()
    {
        // Arrange
        var item = CreateSaleItem(quantity: 4, unitPrice: 200m);

        // Act
        item.CalculateDiscount();

        // Assert
        item.Discount.Should().Be(0.10m);
        item.TotalAmount.Should().Be(4 * 200m * 0.90m);
    }
}

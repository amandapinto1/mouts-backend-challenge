using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.TestData;

public static class SaleTestData
{
    private static readonly Faker<CreateSaleCommand> CreateSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.SaleNumber, f => f.Random.AlphaNumeric(10).ToUpper())
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.Items, f => GenerateItems(f.Random.Int(1, 5)));

    private static readonly Faker<CreateSaleItemCommand> CreateSaleItemCommandFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(10, 500));

    public static CreateSaleCommand GenerateValidCommand() => CreateSaleCommandFaker.Generate();

    public static CreateSaleCommand GenerateCommandWithInvalidQuantity()
    {
        var command = CreateSaleCommandFaker.Generate();
        command.Items =
        [
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                Quantity = 21,
                UnitPrice = 100m
            }
        ];
        return command;
    }

    public static List<CreateSaleItemCommand> GenerateItems(int count) =>
        CreateSaleItemCommandFaker.Generate(count);

    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.Id, f => f.Random.Guid())
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Finance.Amount(10, 500))
        .RuleFor(i => i.IsCancelled, false);

    public static Sale GenerateSale(int itemCount = 3)
    {
        var faker = new Faker();
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = faker.Random.AlphaNumeric(10).ToUpper(),
            SaleDate = faker.Date.Recent(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = SaleItemFaker.Generate(itemCount)
        };

        foreach (var item in sale.Items)
            item.CalculateDiscount();

        sale.CalculateTotalAmount();
        return sale;
    }
}

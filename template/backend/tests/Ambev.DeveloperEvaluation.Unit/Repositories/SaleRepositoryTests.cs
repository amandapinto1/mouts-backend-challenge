using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Repositories;

public class SaleRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;
    private readonly Faker _faker = new();

    public SaleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
    }

    [Fact(DisplayName = "Given valid sale When creating Then returns sale with generated Id")]
    public async Task CreateAsync_ValidSale_ReturnsSaleWithId()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = _faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            TotalAmount = 500m,
            Items =
            [
                new SaleItem
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                    UnitPrice = 100m,
                    Discount = 0.10m,
                    TotalAmount = 450m
                }
            ]
        };

        // Act
        var result = await _repository.CreateAsync(sale);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.SaleNumber.Should().Be(sale.SaleNumber);
        result.Items.Should().HaveCount(1);
    }

    [Fact(DisplayName = "Given existing sale When getting by Id Then returns sale with items")]
    public async Task GetByIdAsync_ExistingSale_ReturnsSaleWithItems()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = _faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            TotalAmount = 200m,
            Items =
            [
                new SaleItem
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 100m,
                    TotalAmount = 200m
                }
            ]
        };
        await _repository.CreateAsync(sale);

        // Act
        var result = await _repository.GetByIdAsync(sale.Id);

        // Assert
        result.Should().NotBeNull();
        result!.SaleNumber.Should().Be(sale.SaleNumber);
        result.Items.Should().HaveCount(1);
    }

    [Fact(DisplayName = "Given non-existent Id When getting by Id Then returns null")]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact(DisplayName = "Given existing sale When updating Then persists changes")]
    public async Task UpdateAsync_ExistingSale_PersistsChanges()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            TotalAmount = 100m,
            Items = []
        };
        await _repository.CreateAsync(sale);

        // Act
        sale.TotalAmount = 999m;
        await _repository.UpdateAsync(sale);

        // Assert
        var updated = await _repository.GetByIdAsync(sale.Id);
        updated.TotalAmount.Should().Be(999m);
    }

    [Fact(DisplayName = "Given existing sale When deleting Then returns true and removes sale")]
    public async Task DeleteAsync_ExistingSale_ReturnsTrueAndRemoves()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "SALE-DEL",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            TotalAmount = 0m,
            Items = []
        };
        await _repository.CreateAsync(sale);

        // Act
        var result = await _repository.DeleteAsync(sale.Id);

        // Assert
        result.Should().BeTrue();
        var deleted = await _repository.GetByIdAsync(sale.Id);
        deleted.Should().BeNull();
    }

    [Fact(DisplayName = "Given non-existent Id When deleting Then returns false")]
    public async Task DeleteAsync_NonExistentId_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given multiple sales When getting all Then returns queryable")]
    public async Task GetAllAsync_MultipleSales_ReturnsAll()
    {
        // Arrange
        for (int i = 0; i < 3; i++)
        {
            await _repository.CreateAsync(new Sale
            {
                SaleNumber = $"SALE-{i}",
                SaleDate = DateTime.UtcNow,
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                TotalAmount = i * 100m,
                Items = []
            });
        }

        // Act
        var query = await _repository.GetAllAsync();
        var result = await query.ToListAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    public void Dispose() => _context.Dispose();
}

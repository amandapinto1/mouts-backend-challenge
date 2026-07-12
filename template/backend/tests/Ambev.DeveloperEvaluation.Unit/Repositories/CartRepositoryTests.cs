using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Repositories;

public class CartRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly CartRepository _repository;
    private readonly Faker _faker = new();

    public CartRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new CartRepository(_context);
    }

    [Fact(DisplayName = "Given valid cart When creating Then returns cart with Id")]
    public async Task CreateAsync_ValidCart_ReturnsWithId()
    {
        // Arrange
        var cart = new Cart
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products =
            [
                new CartItem { ProductId = Guid.NewGuid(), Quantity = 3 },
                new CartItem { ProductId = Guid.NewGuid(), Quantity = 1 }
            ]
        };

        // Act
        var result = await _repository.CreateAsync(cart);

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Products.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Given existing cart When getting by Id Then returns cart with products")]
    public async Task GetByIdAsync_ExistingCart_ReturnsCartWithProducts()
    {
        // Arrange
        var cart = new Cart
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new CartItem { ProductId = Guid.NewGuid(), Quantity = 5 }]
        };
        await _repository.CreateAsync(cart);

        // Act
        var result = await _repository.GetByIdAsync(cart.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Products.Should().HaveCount(1);
        result.Products[0].Quantity.Should().Be(5);
    }

    [Fact(DisplayName = "Given existing cart When updating Then persists changes")]
    public async Task UpdateAsync_ExistingCart_PersistsChanges()
    {
        // Arrange
        var cart = new Cart
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new CartItem { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };
        await _repository.CreateAsync(cart);

        // Act
        cart.Products[0].Quantity = 10;
        await _repository.UpdateAsync(cart);

        // Assert
        var updated = await _repository.GetByIdAsync(cart.Id);
        updated!.Products[0].Quantity.Should().Be(10);
    }

    [Fact(DisplayName = "Given existing cart When deleting Then returns true")]
    public async Task DeleteAsync_ExistingCart_ReturnsTrue()
    {
        // Arrange
        var cart = new Cart { UserId = Guid.NewGuid(), Date = DateTime.UtcNow, Products = [] };
        await _repository.CreateAsync(cart);

        // Act
        var result = await _repository.DeleteAsync(cart.Id);

        // Assert
        result.Should().BeTrue();
        var deleted = await _repository.GetByIdAsync(cart.Id);
        deleted.Should().BeNull();
    }

    [Fact(DisplayName = "Given non-existent Id When deleting Then returns false")]
    public async Task DeleteAsync_NonExistent_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(Guid.NewGuid());

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "Given multiple carts When getting all Then returns all carts")]
    public async Task GetAllAsync_MultipleCarts_ReturnsAll()
    {
        // Arrange
        for (int i = 0; i < 4; i++)
        {
            await _repository.CreateAsync(new Cart
            {
                UserId = Guid.NewGuid(),
                Date = DateTime.UtcNow.AddDays(-i),
                Products = []
            });
        }

        // Act
        var query = await _repository.GetAllAsync();
        var result = await query.ToListAsync();

        // Assert
        result.Should().HaveCount(4);
    }

    public void Dispose() => _context.Dispose();
}

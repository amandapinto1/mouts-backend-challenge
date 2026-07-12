using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly Faker _faker = new();

    public ProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
    }

    [Fact(DisplayName = "Given valid product command When creating Then returns product result")]
    public async Task CreateProduct_ValidCommand_ReturnsResult()
    {
        // Arrange
        var handler = new CreateProductHandler(_productRepository, _mapper);
        var command = new CreateProductCommand
        {
            Title = _faker.Commerce.ProductName(),
            Price = 99.99m,
            Description = _faker.Commerce.ProductDescription(),
            CategoryId = Guid.NewGuid(),
            Image = _faker.Internet.Url(),
            RatingRate = 4.5m,
            RatingCount = 10
        };

        var product = new Product { Id = Guid.NewGuid(), Title = command.Title, Price = command.Price };
        var expectedResult = new CreateProductResult { Id = product.Id, Title = product.Title };

        _mapper.Map<Product>(command).Returns(product);
        _productRepository.CreateAsync(product, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<CreateProductResult>(product).Returns(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        await _productRepository.Received(1).CreateAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given existing product When getting by Id Then returns product")]
    public async Task GetProduct_ExistingId_ReturnsProduct()
    {
        // Arrange
        var handler = new GetProductHandler(_productRepository, _mapper);
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Title = "Test Product" };
        var expectedResult = new GetProductResult { Id = productId, Title = "Test Product" };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<GetProductResult>(product).Returns(expectedResult);

        // Act
        var result = await handler.Handle(new GetProductCommand { Id = productId }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
    }

    [Fact(DisplayName = "Given non-existent product When getting by Id Then throws KeyNotFoundException")]
    public async Task GetProduct_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new GetProductHandler(_productRepository, _mapper);
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Product?)null);

        // Act
        var act = async () => await handler.Handle(new GetProductCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given existing product When updating Then returns updated product")]
    public async Task UpdateProduct_ValidCommand_ReturnsUpdatedProduct()
    {
        // Arrange
        var handler = new UpdateProductHandler(_productRepository, _mapper);
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Title = "Old Title", Price = 50m };

        var command = new UpdateProductCommand
        {
            Id = productId,
            Title = "New Title",
            Price = 75m,
            Description = "Updated",
            CategoryId = Guid.NewGuid(),
            Image = _faker.Internet.Url(),
            RatingRate = 4.0m,
            RatingCount = 20
        };

        var expectedResult = new UpdateProductResult { Id = productId, Title = "New Title" };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _productRepository.UpdateAsync(product, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<UpdateProductResult>(product).Returns(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Title");
        product.Title.Should().Be("New Title");
        product.Price.Should().Be(75m);
    }

    [Fact(DisplayName = "Given non-existent product When updating Then throws KeyNotFoundException")]
    public async Task UpdateProduct_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new UpdateProductHandler(_productRepository, _mapper);
        _productRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Product?)null);

        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Price = 10m,
            Description = "Test",
            CategoryId = Guid.NewGuid(),
            Image = "img.png"
        };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given existing product When deleting Then returns success")]
    public async Task DeleteProduct_ExistingId_ReturnsSuccess()
    {
        // Arrange
        var handler = new DeleteProductHandler(_productRepository);
        var productId = Guid.NewGuid();
        _productRepository.DeleteAsync(productId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await handler.Handle(new DeleteProductCommand { Id = productId }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Contain("deleted");
    }

    [Fact(DisplayName = "Given non-existent product When deleting Then throws KeyNotFoundException")]
    public async Task DeleteProduct_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new DeleteProductHandler(_productRepository);
        _productRepository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await handler.Handle(new DeleteProductCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

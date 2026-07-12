using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly Faker _faker = new();

    public CartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
    }

    [Fact(DisplayName = "Given valid cart command When creating Then returns cart result")]
    public async Task CreateCart_ValidCommand_ReturnsResult()
    {
        // Arrange
        var handler = new CreateCartHandler(_cartRepository, _mapper);
        var command = new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new CreateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 2 }]
        };

        var cart = new Cart { Id = Guid.NewGuid(), UserId = command.UserId, Date = command.Date };
        var expectedResult = new CreateCartResult { Id = cart.Id };

        _mapper.Map<Cart>(command).Returns(cart);
        _cartRepository.CreateAsync(cart, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<CreateCartResult>(cart).Returns(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cart.Id);
        await _cartRepository.Received(1).CreateAsync(cart, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given existing cart When getting by Id Then returns cart")]
    public async Task GetCart_ExistingId_ReturnsCart()
    {
        // Arrange
        var handler = new GetCartHandler(_cartRepository, _mapper);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid() };
        var expectedResult = new GetCartResult { Id = cartId };

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<GetCartResult>(cart).Returns(expectedResult);

        // Act
        var result = await handler.Handle(new GetCartCommand { Id = cartId }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cartId);
    }

    [Fact(DisplayName = "Given non-existent cart When getting by Id Then throws KeyNotFoundException")]
    public async Task GetCart_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new GetCartHandler(_cartRepository, _mapper);
        _cartRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Cart?)null);

        // Act
        var act = async () => await handler.Handle(new GetCartCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given existing cart When updating Then returns updated cart")]
    public async Task UpdateCart_ValidCommand_ReturnsUpdatedCart()
    {
        // Arrange
        var handler = new UpdateCartHandler(_cartRepository, _mapper);
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid() };

        var command = new UpdateCartCommand
        {
            Id = cartId,
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 3 }]
        };

        var expectedResult = new UpdateCartResult { Id = cartId };

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<List<CartItem>>(command.Products).Returns([new CartItem { ProductId = Guid.NewGuid(), Quantity = 3 }]);
        _cartRepository.UpdateAsync(cart, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<UpdateCartResult>(cart).Returns(expectedResult);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cartId);
        await _cartRepository.Received(1).UpdateAsync(cart, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent cart When updating Then throws KeyNotFoundException")]
    public async Task UpdateCart_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new UpdateCartHandler(_cartRepository, _mapper);
        _cartRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Cart?)null);

        var command = new UpdateCartCommand { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Date = DateTime.UtcNow, Products = [new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }] };

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given existing cart When deleting Then returns success")]
    public async Task DeleteCart_ExistingId_ReturnsSuccess()
    {
        // Arrange
        var handler = new DeleteCartHandler(_cartRepository);
        var cartId = Guid.NewGuid();
        _cartRepository.DeleteAsync(cartId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await handler.Handle(new DeleteCartCommand { Id = cartId }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Message.Should().Contain("deleted");
    }

    [Fact(DisplayName = "Given non-existent cart When deleting Then throws KeyNotFoundException")]
    public async Task DeleteCart_NotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var handler = new DeleteCartHandler(_cartRepository);
        _cartRepository.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var act = async () => await handler.Handle(new DeleteCartCommand { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

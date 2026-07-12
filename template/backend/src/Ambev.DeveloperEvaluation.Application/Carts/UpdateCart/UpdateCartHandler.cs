using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, UpdateCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public UpdateCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cart = await _cartRepository.GetByIdAsync(command.Id, cancellationToken) 
            ?? throw new KeyNotFoundException($"Cart with IdD {command.Id} not found");

        cart.UserId = command.UserId;
        cart.Date = command.Date;
        cart.Products = _mapper.Map<List<CartItem>>(command.Products);

        var updated = await _cartRepository.UpdateAsync(cart, cancellationToken);
        return _mapper.Map<UpdateCartResult>(updated);
    }
}

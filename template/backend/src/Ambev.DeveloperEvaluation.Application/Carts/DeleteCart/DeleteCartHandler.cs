using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

public class DeleteCartHandler : IRequestHandler<DeleteCartCommand, DeleteCartResult>
{
    private readonly ICartRepository _cartRepository;

    public DeleteCartHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<DeleteCartResult> Handle(DeleteCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var deleted = await _cartRepository.DeleteAsync(command.Id, cancellationToken);
        return !deleted
            ? throw new KeyNotFoundException($"Cart with Id {command.Id} not found")
            : new DeleteCartResult { Message = "Cart deleted successfully" };
    }
}

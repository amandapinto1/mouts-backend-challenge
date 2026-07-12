using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartCommandValidator : AbstractValidator<UpdateCartCommand>
{
    public UpdateCartCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Products).NotEmpty().WithMessage("Cart must have at least one product.");
        RuleForEach(x => x.Products).SetValidator(new UpdateCartItemCommandValidator());
    }
}

public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

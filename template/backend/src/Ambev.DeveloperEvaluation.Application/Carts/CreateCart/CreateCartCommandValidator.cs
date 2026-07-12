using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartCommandValidator : AbstractValidator<CreateCartCommand>
{
    public CreateCartCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Products).NotEmpty().WithMessage("Cart must have at least one product.");
        RuleForEach(x => x.Products).SetValidator(new CreateCartItemCommandValidator());
    }
}

public class CreateCartItemCommandValidator : AbstractValidator<CreateCartItemCommand>
{
    public CreateCartItemCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}

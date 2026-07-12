using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("Sale must have at least one item.");
        RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemCommandValidator());
    }
}

public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(20)
            .WithMessage("Quantity must be between 1 and 20.");
    }
}

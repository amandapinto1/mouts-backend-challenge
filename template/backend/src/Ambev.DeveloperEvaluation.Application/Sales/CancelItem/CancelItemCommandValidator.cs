using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemCommandValidator : AbstractValidator<CancelItemCommand>
{
    public CancelItemCommandValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
    }
}

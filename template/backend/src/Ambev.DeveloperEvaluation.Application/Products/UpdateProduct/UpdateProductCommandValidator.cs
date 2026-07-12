using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Image).MaximumLength(500);
        RuleFor(x => x.RatingRate).InclusiveBetween(0, 5);
        RuleFor(x => x.RatingCount).GreaterThanOrEqualTo(0);
    }
}

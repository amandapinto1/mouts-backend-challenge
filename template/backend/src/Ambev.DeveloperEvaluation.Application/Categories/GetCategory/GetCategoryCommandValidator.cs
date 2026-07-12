using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Categories.GetCategory;

public class GetCategoryCommandValidator : AbstractValidator<GetCategoryCommand>
{
    public GetCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

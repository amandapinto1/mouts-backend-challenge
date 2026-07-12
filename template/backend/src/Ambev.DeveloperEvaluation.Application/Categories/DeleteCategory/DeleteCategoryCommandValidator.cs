using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Categories.DeleteCategory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

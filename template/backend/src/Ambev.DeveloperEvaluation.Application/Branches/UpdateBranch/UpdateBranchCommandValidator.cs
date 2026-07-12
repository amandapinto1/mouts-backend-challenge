using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Branches.UpdateBranch;

public class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Address).MaximumLength(300);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.State).MaximumLength(100);
    }
}

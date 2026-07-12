using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Categories.DeleteCategory;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, DeleteCategoryResult>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<DeleteCategoryResult> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var validator = new DeleteCategoryCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var deleted = await _categoryRepository.DeleteAsync(command.Id, cancellationToken);
        if (!deleted)
            throw new KeyNotFoundException($"Category with Id {command.Id} not found");

        return new DeleteCategoryResult { Message = "Category deleted successfully" };
    }
}

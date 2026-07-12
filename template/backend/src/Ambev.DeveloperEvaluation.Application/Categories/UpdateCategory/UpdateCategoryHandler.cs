using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Categories.UpdateCategory;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<UpdateCategoryResult> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateCategoryCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Category with Id {command.Id} not found");

        category.Name = command.Name;
        category.Description = command.Description;
        category.IsActive = command.IsActive;

        var updated = await _categoryRepository.UpdateAsync(category, cancellationToken);
        return _mapper.Map<UpdateCategoryResult>(updated);
    }
}

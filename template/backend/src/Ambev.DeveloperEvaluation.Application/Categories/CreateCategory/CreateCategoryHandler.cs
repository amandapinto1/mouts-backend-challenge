using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Categories.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CreateCategoryResult> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCategoryCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var category = _mapper.Map<Category>(command);
        var created = await _categoryRepository.CreateAsync(category, cancellationToken);
        return _mapper.Map<CreateCategoryResult>(created);
    }
}

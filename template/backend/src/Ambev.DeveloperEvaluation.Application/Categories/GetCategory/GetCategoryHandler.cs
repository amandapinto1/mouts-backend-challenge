using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Categories.GetCategory;

public class GetCategoryHandler : IRequestHandler<GetCategoryCommand, GetCategoryResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<GetCategoryResult> Handle(GetCategoryCommand command, CancellationToken cancellationToken)
    {
        var validator = new GetCategoryCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);
        return category == null
            ? throw new KeyNotFoundException($"Category with Id {command.Id} not found")
            : _mapper.Map<GetCategoryResult>(category);
    }
}

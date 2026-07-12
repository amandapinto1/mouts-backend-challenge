using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.Application.Categories.ListCategories;

public class ListCategoriesHandler : IRequestHandler<ListCategoriesCommand, ListCategoriesResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public ListCategoriesHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ListCategoriesResult> Handle(ListCategoriesCommand command, CancellationToken cancellationToken)
    {
        var query = await _categoryRepository.GetAllAsync(cancellationToken);

        query = query.ApplyStringFilter(c => c.Name, command.Name);

        if (command.IsActive.HasValue)
            query = query.Where(c => c.IsActive == command.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(command.Order))
            query = query.OrderBy(command.Order);

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)command.Size);

        var items = await query
            .Skip((command.Page - 1) * command.Size)
            .Take(command.Size)
            .ToListAsync(cancellationToken);

        return new ListCategoriesResult
        {
            Data = _mapper.Map<List<ListCategoriesItemResult>>(items),
            TotalItems = totalItems,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}

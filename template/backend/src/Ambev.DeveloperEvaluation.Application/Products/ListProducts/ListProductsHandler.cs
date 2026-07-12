using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsHandler : IRequestHandler<ListProductsCommand, ListProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ListProductsResult> Handle(ListProductsCommand command, CancellationToken cancellationToken)
    {
        var query = await _productRepository.GetAllAsync(cancellationToken);

        // Apply filters
        query = query.ApplyStringFilter(p => p.Title, command.Title);
        if (command.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == command.CategoryId.Value);
        if (command.MinPrice.HasValue)
            query = query.Where(p => p.Price >= command.MinPrice.Value);
        if (command.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= command.MaxPrice.Value);

        if (!string.IsNullOrWhiteSpace(command.Order))
            query = query.OrderBy(command.Order);

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)command.Size);

        var items = await query
            .Skip((command.Page - 1) * command.Size)
            .Take(command.Size)
            .ToListAsync(cancellationToken);

        return new ListProductsResult
        {
            Data = _mapper.Map<List<ListProductsItemResult>>(items),
            TotalItems = totalItems,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}

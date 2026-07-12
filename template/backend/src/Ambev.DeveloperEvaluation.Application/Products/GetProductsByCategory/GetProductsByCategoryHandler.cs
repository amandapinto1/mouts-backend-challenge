using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProductsByCategory;

public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryCommand, GetProductsByCategoryResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsByCategoryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryCommand command, CancellationToken cancellationToken)
    {
        var query = await _productRepository.GetByCategoryAsync(command.Category, cancellationToken);

        if (!string.IsNullOrWhiteSpace(command.Order))
            query = query.OrderBy(command.Order);

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)command.Size);

        var items = await query
            .Skip((command.Page - 1) * command.Size)
            .Take(command.Size)
            .ToListAsync(cancellationToken);

        return new GetProductsByCategoryResult
        {
            Data = _mapper.Map<List<ListProductsItemResult>>(items),
            TotalItems = totalItems,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}

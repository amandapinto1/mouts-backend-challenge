using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsHandler : IRequestHandler<ListCartsCommand, ListCartsResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public ListCartsHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<ListCartsResult> Handle(ListCartsCommand command, CancellationToken cancellationToken)
    {
        var query = await _cartRepository.GetAllAsync(cancellationToken);

        // Apply filters
        if (command.UserId.HasValue)
            query = query.Where(c => c.UserId == command.UserId.Value);
        if (command.MinDate.HasValue)
            query = query.Where(c => c.Date >= command.MinDate.Value);
        if (command.MaxDate.HasValue)
            query = query.Where(c => c.Date <= command.MaxDate.Value);

        if (!string.IsNullOrWhiteSpace(command.Order))
            query = query.OrderBy(command.Order);

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)command.Size);

        var items = await query
            .Skip((command.Page - 1) * command.Size)
            .Take(command.Size)
            .ToListAsync(cancellationToken);

        return new ListCartsResult
        {
            Data = _mapper.Map<List<ListCartsItemResult>>(items),
            TotalItems = totalItems,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}

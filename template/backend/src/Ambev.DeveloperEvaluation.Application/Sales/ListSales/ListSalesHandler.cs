using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<ListSalesResult> Handle(ListSalesCommand command, CancellationToken cancellationToken)
    {
        var query = await _saleRepository.GetAllAsync(cancellationToken);

        // Apply filters
        query = query.ApplyStringFilter(s => s.SaleNumber, command.SaleNumber);
        if (command.CustomerId.HasValue)
            query = query.Where(s => s.CustomerId == command.CustomerId.Value);
        query = query.ApplyStringFilter(s => s.CustomerName, command.CustomerName);
        if (command.BranchId.HasValue)
            query = query.Where(s => s.BranchId == command.BranchId.Value);
        query = query.ApplyStringFilter(s => s.BranchName, command.BranchName);
        if (command.MinDate.HasValue)
            query = query.Where(s => s.SaleDate >= command.MinDate.Value);
        if (command.MaxDate.HasValue)
            query = query.Where(s => s.SaleDate <= command.MaxDate.Value);
        if (command.MinTotal.HasValue)
            query = query.Where(s => s.TotalAmount >= command.MinTotal.Value);
        if (command.MaxTotal.HasValue)
            query = query.Where(s => s.TotalAmount <= command.MaxTotal.Value);
        if (command.IsCancelled.HasValue)
            query = query.Where(s => s.IsCancelled == command.IsCancelled.Value);

        if (!string.IsNullOrWhiteSpace(command.Order))
            query = query.OrderBy(command.Order);

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)command.Size);

        var items = await query
            .Skip((command.Page - 1) * command.Size)
            .Take(command.Size)
            .ToListAsync(cancellationToken);

        return new ListSalesResult
        {
            Data = _mapper.Map<List<ListSalesItemResult>>(items),
            TotalItems = totalItems,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}

using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.Application.Branches.ListBranches;

public class ListBranchesHandler : IRequestHandler<ListBranchesCommand, ListBranchesResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public ListBranchesHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<ListBranchesResult> Handle(ListBranchesCommand command, CancellationToken cancellationToken)
    {
        var query = await _branchRepository.GetAllAsync(cancellationToken);

        query = query.ApplyStringFilter(b => b.Name, command.Name);
        query = query.ApplyStringFilter(b => b.City, command.City);
        query = query.ApplyStringFilter(b => b.State, command.State);

        if (command.IsActive.HasValue)
            query = query.Where(b => b.IsActive == command.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(command.Order))
            query = query.OrderBy(command.Order);

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)command.Size);

        var items = await query
            .Skip((command.Page - 1) * command.Size)
            .Take(command.Size)
            .ToListAsync(cancellationToken);

        return new ListBranchesResult
        {
            Data = _mapper.Map<List<ListBranchesItemResult>>(items),
            TotalItems = totalItems,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}

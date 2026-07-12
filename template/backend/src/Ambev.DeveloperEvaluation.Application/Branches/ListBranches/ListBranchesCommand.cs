using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.ListBranches;

public class ListBranchesCommand : IRequest<ListBranchesResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Order { get; set; }
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public bool? IsActive { get; set; }
}

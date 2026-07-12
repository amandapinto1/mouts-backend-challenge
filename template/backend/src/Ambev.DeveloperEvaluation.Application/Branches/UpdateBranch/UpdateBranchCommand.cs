using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.UpdateBranch;

public class UpdateBranchCommand : IRequest<UpdateBranchResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

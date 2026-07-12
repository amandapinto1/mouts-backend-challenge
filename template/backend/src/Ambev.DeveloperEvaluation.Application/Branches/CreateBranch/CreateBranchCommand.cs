using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;

public class CreateBranchCommand : IRequest<CreateBranchResult>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}

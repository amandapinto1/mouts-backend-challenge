using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Branches.ListBranches;

public class ListBranchesProfile : Profile
{
    public ListBranchesProfile()
    {
        CreateMap<Branch, ListBranchesItemResult>();
    }
}

using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Categories.GetCategory;

public class GetCategoryProfile : Profile
{
    public GetCategoryProfile()
    {
        CreateMap<Category, GetCategoryResult>();
    }
}

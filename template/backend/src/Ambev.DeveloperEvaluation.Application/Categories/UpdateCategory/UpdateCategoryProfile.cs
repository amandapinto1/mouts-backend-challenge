using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Categories.UpdateCategory;

public class UpdateCategoryProfile : Profile
{
    public UpdateCategoryProfile()
    {
        CreateMap<Category, UpdateCategoryResult>();
    }
}

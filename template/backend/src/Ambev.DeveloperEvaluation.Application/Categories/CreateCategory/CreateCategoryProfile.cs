using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Categories.CreateCategory;

public class CreateCategoryProfile : Profile
{
    public CreateCategoryProfile()
    {
        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<Category, CreateCategoryResult>();
    }
}

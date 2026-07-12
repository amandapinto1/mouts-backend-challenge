using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Categories.ListCategories;

public class ListCategoriesProfile : Profile
{
    public ListCategoriesProfile()
    {
        CreateMap<Category, ListCategoriesItemResult>();
    }
}

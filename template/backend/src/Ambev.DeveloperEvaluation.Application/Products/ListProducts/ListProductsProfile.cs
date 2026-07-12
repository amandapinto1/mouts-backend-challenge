using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Products.Common;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsProfile : Profile
{
    public ListProductsProfile()
    {
        CreateMap<Product, ListProductsItemResult>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new RatingDto { Rate = src.RatingRate, Count = src.RatingCount }));
    }
}

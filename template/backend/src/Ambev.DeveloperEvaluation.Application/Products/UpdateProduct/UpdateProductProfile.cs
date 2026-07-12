using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Products.Common;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public class UpdateProductProfile : Profile
{
    public UpdateProductProfile()
    {
        CreateMap<UpdateProductCommand, Product>();
        CreateMap<Product, UpdateProductResult>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new RatingDto { Rate = src.RatingRate, Count = src.RatingCount }));
    }
}

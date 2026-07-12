using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        CreateMap<CreateSaleItemCommand, SaleItem>();
        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, CreateSaleItemResult>();
    }
}

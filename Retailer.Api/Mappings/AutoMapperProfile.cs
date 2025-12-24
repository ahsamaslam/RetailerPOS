using AutoMapper;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Mappings;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateItemDto, Item>();
        CreateMap<Item, ItemDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.GroupName, o => o.MapFrom(s => s.Group != null ? s.Group.Name : null))
            .ForMember(d => d.SubGroupName, o => o.MapFrom(s => s.SubGroup != null ? s.SubGroup.Name : null))
            .ForMember(d => d.QtyInHand, o => o.MapFrom(s => s.QtyInHand ));

        CreateMap<CreatePurchaseDto, PurchaseMaster>();
        CreateMap<CreatePurchaseDetailDto, PurchaseDetail>();
        CreateMap<PurchaseMaster, PurchaseMasterDto>();
        CreateMap<PurchaseDetail, PurchaseDetailDto>();
        CreateMap<CustomerPayment, CustomerPaymentDto>();
    }
}

using AutoMapper;
using Retailer.Api.DtoReport;
using Retailer.Api.DTOs;
using Retailer.Api.Entities;
using Retailer.POS.Api.DTOs;
using Retailer.POS.Api.Entities;

namespace Retailer.POS.Api.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // ---------- Items ----------
        CreateMap<CreateItemDto, Item>();
        CreateMap<Item, ItemDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.GroupName, o => o.MapFrom(s => s.Group != null ? s.Group.Name : null))
            .ForMember(d => d.SubGroupName, o => o.MapFrom(s => s.SubGroup != null ? s.SubGroup.Name : null))
            .ForMember(d => d.QtyInHand, o => o.MapFrom(s => s.QtyInHand));

        // ---------- Purchase ----------
        CreateMap<CreatePurchaseDto, PurchaseMaster>();
        CreateMap<CreatePurchaseDetailDto, PurchaseDetail>();
        
        CreateMap<PurchaseMaster, PurchaseMasterDto>();
        CreateMap<PurchaseDetail, PurchaseDetailDto>();


        CreateMap<SalesMaster, SalesMasterDto>();
        CreateMap<SalesDetail, SalesDetailDto>();

        // ---------- Purchase Return (CREATE) ----------
        CreateMap<CreatePurchaseReturnDto, PurchaseReturnMaster>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Vendor, o => o.Ignore())
            .ForMember(d => d.Branch, o => o.Ignore())
            .ForMember(d => d.remarks, o => o.Ignore())
            .ForMember(d => d.Details, o => o.MapFrom(s => s.Details));

        CreateMap<CreatePurchaseReturnDetailDto, PurchaseReturnDetail>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.PurchaseReturnId, o => o.Ignore())
            .ForMember(d => d.Purchase, o => o.Ignore())
            .ForMember(d => d.Item, o => o.Ignore());

        // ---------- Purchase Return (READ) ----------
        CreateMap<PurchaseReturnMaster, PurchaseReturnMasterDto>()
            .ForMember(d => d.Remarks, o => o.MapFrom(s => s.remarks))
            .ForMember(d => d.VendorName, o => o.MapFrom(s => s.Vendor != null ? s.Vendor.Name : null));

        CreateMap<PurchaseReturnDetail, PurchaseReturnDetailDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.Name : null));

        CreateMap<PurchaseDetail, ItemPurchaseReportDtoR>()
           .ForMember(dest => dest.srno, opt => opt.Ignore())
           .ForMember(dest => dest.productCode, opt => opt.MapFrom(src => src.ItemId))
           .ForMember(dest => dest.productName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : string.Empty))
           .ForMember(dest => dest.purchaseID, opt => opt.MapFrom(src => src.PurchaseId))
           .ForMember(dest => dest.purchaseDate, opt => opt.MapFrom(src => src.Purchase != null ? src.Purchase.Date : DateTime.MinValue))
           .ForMember(dest => dest.vendorName, opt => opt.MapFrom(src => src.Purchase != null && src.Purchase.Vendor != null ? src.Purchase.Vendor.Name : string.Empty))
           .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.Qty))
           .ForMember(dest => dest.unitPrice, opt => opt.MapFrom(src => src.Rate))
           .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Discount))
           .ForMember(dest => dest.taxAmount, opt => opt.MapFrom(src => src.TaxAmount))
           .ForMember(dest => dest.subTotal, opt => opt.MapFrom(src => (src.Qty * src.Rate - src.Discount) + src.TaxAmount));


        CreateMap<PurchaseReturnDetail, ItemPurchaseReturnReportDtoR>()
           .ForMember(dest => dest.srno, opt => opt.Ignore())
           .ForMember(dest => dest.productCode, opt => opt.MapFrom(src => src.ItemId))
           .ForMember(dest => dest.productName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : string.Empty))
           .ForMember(dest => dest.purchaseReturnID, opt => opt.MapFrom(src => src.PurchaseReturnId))
           .ForMember(dest => dest.purchaseReturnDate, opt => opt.MapFrom(src => src.Purchase != null ? src.Purchase.Date : DateTime.MinValue))
           .ForMember(dest => dest.vendorName, opt => opt.MapFrom(src => src.Purchase != null && src.Purchase.Vendor != null ? src.Purchase.Vendor.Name : string.Empty))
           .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.Qty))
           .ForMember(dest => dest.unitPrice, opt => opt.MapFrom(src => src.Rate))
           .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Discount))
           .ForMember(dest => dest.taxAmount, opt => opt.MapFrom(src => src.TaxAmount))
           .ForMember(dest => dest.subTotal, opt => opt.MapFrom(src => (src.Qty * src.Rate - src.Discount) + src.TaxAmount));



        CreateMap<SalesReturnMaster, SalesReturnMasterDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.UserName, o => o.Ignore()) // comes from elsewhere (identity)
            .ForMember(d => d.Details, o => o.MapFrom(s => s.Details));

        CreateMap<SalesReturnMasterDto, SalesReturnMaster>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.CreateDate, o => o.Ignore())
            .ForMember(d => d.Active, o => o.Ignore())
            .ForMember(d => d.Year, o => o.Ignore())
            .ForMember(d => d.saleCode, o => o.Ignore())
            .ForMember(d => d.totalAmount, o => o.Ignore())
            .ForMember(d => d.remarks, o => o.Ignore())
            .ForMember(d => d.hsCode, o => o.Ignore())
            .ForMember(d => d.Details, o => o.MapFrom(s => s.Details));

        // Detail mapping
        CreateMap<SalesReturnDetail, SalesReturnDetailDto>();

        CreateMap<SalesReturnDetailDto, SalesReturnDetail>()
            .ForMember(d => d.SalesReturnMasterId, o => o.Ignore())
            .ForMember(d => d.SalesReturnMaster, o => o.Ignore())
            .ForMember(d => d.sroScheduleNo, o => o.Ignore())
            .ForMember(d => d.uoM, o => o.Ignore())
            .ForMember(d => d.hsCode, o => o.Ignore())
            .ForMember(d => d.otherTax, o => o.Ignore())
            .ForMember(d => d.fedPayable, o => o.Ignore())
            .ForMember(d => d.saleType, o => o.Ignore())
            .ForMember(d => d.sroItemSerialNo, o => o.Ignore());


        CreateMap<SalesDetail, ItemSalesReportDtoR>()
           .ForMember(dest => dest.srno, opt => opt.Ignore())
           .ForMember(dest => dest.productCode, opt => opt.MapFrom(src => src.ItemCode))
           .ForMember(dest => dest.productName, opt => opt.MapFrom(src => src.ItemName))
           .ForMember(dest => dest.salesID, opt => opt.MapFrom(src => src.SalesMasterId))
           .ForMember(dest => dest.salesDate, opt => opt.MapFrom(src => src.SalesMaster != null ? src.SalesMaster.Date : DateTime.MinValue))
           .ForMember(dest => dest.customerName, opt => opt.MapFrom(src => src.SalesMaster != null ? src.SalesMaster.CustomerName : string.Empty))
           .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.Qty))
           .ForMember(dest => dest.unitPrice, opt => opt.MapFrom(src => src.Rate))
           .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Discount))
           .ForMember(dest => dest.taxAmount, opt => opt.MapFrom(src => src.TaxAmount))
           .ForMember(dest => dest.subTotal, opt => opt.MapFrom(src => (src.Qty * src.Rate - src.Discount) + src.TaxAmount));


        CreateMap<SalesReturnDetail, ItemSalesReturnReportDtoR>()
           .ForMember(dest => dest.srno, opt => opt.Ignore())
           .ForMember(dest => dest.productCode, opt => opt.MapFrom(src => src.ItemCode))
           .ForMember(dest => dest.productName, opt => opt.MapFrom(src => src.ItemName))
           .ForMember(dest => dest.salesReturnID, opt => opt.MapFrom(src => src.SalesReturnMasterId))
           .ForMember(dest => dest.salesReturnDate, opt => opt.MapFrom(src => src.SalesReturnMaster != null ? src.SalesReturnMaster.Date : DateTime.MinValue))
           .ForMember(dest => dest.customerName, opt => opt.MapFrom(src => src.SalesReturnMaster != null ? src.SalesReturnMaster.CustomerName : string.Empty))
           .ForMember(dest => dest.quantity, opt => opt.MapFrom(src => src.Qty))
           .ForMember(dest => dest.unitPrice, opt => opt.MapFrom(src => src.Rate))
           .ForMember(dest => dest.discount, opt => opt.MapFrom(src => src.Discount))
           .ForMember(dest => dest.taxAmount, opt => opt.MapFrom(src => src.TaxAmount))
           .ForMember(dest => dest.subTotal, opt => opt.MapFrom(src => (src.Qty * src.Rate - src.Discount) + src.TaxAmount));

    }
}

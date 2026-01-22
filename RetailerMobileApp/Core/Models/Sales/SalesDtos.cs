using System;
using System.Collections.Generic;

namespace RetailerMobileApp.Core.Models.Sales;

public class SalesMasterDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public string? CustomerName { get; set; }
    public string? SaleType { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public int? CustomerCode { get; set; }
    public int CustomerID { get; set; }
    public List<SalesDetailDto> Details { get; set; } = new();
}

public class SalesDetailDto
{
    public int Id { get; set; }
    public int ItemCode { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Amount { get; set; }
    public decimal ExtraTaxP { get; set; }
    public decimal FurtherTaxP { get; set; }
    public decimal ExtraTax { get; set; }
    public decimal FurtherTax { get; set; }
}

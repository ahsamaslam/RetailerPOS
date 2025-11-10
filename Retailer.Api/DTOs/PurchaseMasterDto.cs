namespace Retailer.POS.Api.DTOs;
public class PurchaseMasterDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public List<PurchaseDetailDto> Details { get; set; } = new();
}

public class PurchaseDetailDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal Qty { get; set; }
}

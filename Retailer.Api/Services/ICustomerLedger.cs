namespace Retailer.Api.Services
{
    public interface ICustomerLedger
    {
        int Id { get; set; }
        int EntityId { get; set; } // CustomerId, VendorId, BankId
        DateTime Date { get; set; }
        decimal Debit { get; set; }
        decimal Credit { get; set; }
        decimal Balance { get; set; }
        string ReferenceType { get; set; } // Sale, Payment, etc.
        int ReferenceId { get; set; }      // Id of Sale or Payment
        Guid CompanyId { get; set; }
    }

}

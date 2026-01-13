using Retailer.Api.DTOs;
using Retailer.Api.Services;
using Retailer.POS.Api.Entities;
using System.Text.Json;

public class FbrClient : IFbrClient
{
    private readonly HttpClient _http;
    private readonly ILogger<FbrClient> _logger;

    public FbrClient(HttpClient http, ILogger<FbrClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<FbrResult> SendInvoiceAsync(CompanyDto company, SalesMaster sale, Customer customer)
    {
        try
        {
            // build payload according to FBR requirements
            var payload = new
            {
                invoiceType = "Sale Invoice",
                invoiceDate = sale.Date.ToString("yyyy-MM-dd"),
                sellerNTNCNIC = company.NTN,
                sellerBusinessName =company.Name,
                sellerProvince = company.Province,
                sellerAddress = company.Address,
                buyerNTNCNIC = Convert.ToString( customer.CNIC)==""? customer.NTN: customer.CNIC,
                buyerBusinessName = customer.Name,
                buyerProvince = customer.Province,
                buyerAddress = customer.Address,
                buyerRegistrationType =customer.Register==true? "Registered" : "Unregistered",
                invoiceRefNo = "",
                scenarioId = customer.Register == true ? "SN001": "SN002",
                items = sale.Details.Select(d => new
                {
                    hsCode = "2716.0000",
                    productDescription = d.Item.Name,
                    rate = d.TaxPercentage.ToString() + "%",
                    uoM = d.uoM,
                    quantity = d.Qty,
                    totalValues =d.Amount,
                    valueSalesExcludingST =  d.Amount-d.TaxAmount,
                    fixedNotifiedValueOrRetailPrice = 0.0,
                    salesTaxApplicable = d.TaxAmount,
                    salesTaxWithheldAtSource = 0.0,
                    extraTax = d.otherTax,
                    furtherTax = d.furtherTax,
                    sroScheduleNo = d.sroScheduleNo,
                    fedPayable = d.fedPayable,
                    discount = d.Discount,
                    saleType = "Goods at standard rate (default)",
                    sroItemSerialNo = d.sroItemSerialNo
                }).ToArray(), 
            };
            _http.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", company.pralToken);

            // Optionally ensure Accept header is JSON
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var resp = await _http.PostAsJsonAsync("https://gw.fbr.gov.pk/di_data/v1/di/postinvoicedata_sb", payload);
            if (!resp.IsSuccessStatusCode)
            {
                var text = await resp.Content.ReadAsStringAsync();
                _logger.LogWarning("FBR returned failure: {Status} - {Body}", resp.StatusCode, text);
                return new FbrResult(false, Message: text);
            }

            var body = await resp.Content.ReadFromJsonAsync<JsonElement?>();
            // parse external id if any
            var extId = body?.GetProperty("externalId").GetString();
            return new FbrResult(true, ExternalId: "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send invoice to FBR");
            return new FbrResult(false, Message: ex.Message);
        }
    }
}

using Retailer.Api.DtoReport;
using Retailer.Api.DTOs;
using Retailer.POS.Api.DTOs;
using System.Data;

namespace Retailer.Api.Helper
{
    public static class DataTableHelper
    
    {
        public static DataTable ItemWisePurchaseToDataTable(IEnumerable<ItemPurchaseReportDtoR> items)
        {
            var table = new DataTable();

            // Define columns
            table.Columns.Add("SrNo", typeof(int));
            table.Columns.Add("ProductCode", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("PurchaseID", typeof(int));
            table.Columns.Add("PurchaseDate", typeof(DateTime));
            table.Columns.Add("VendorName", typeof(string));
            table.Columns.Add("Quantity", typeof(decimal));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("Discount", typeof(decimal));
            table.Columns.Add("TaxAmount", typeof(decimal));

            // Fill rows
            int srNo = 1;
            foreach (var item in items)
            {
                var row = table.NewRow();
                row["SrNo"] = srNo++;
                row["ProductCode"] = item.productCode;
                row["ProductName"] = item.productName;
                row["PurchaseID"] = item.purchaseID;
                row["PurchaseDate"] = item.purchaseDate;
                row["VendorName"] = item.vendorName;
                row["Quantity"] = item.quantity;
                row["UnitPrice"] = item.unitPrice;
                row["Discount"] = item.discount;
                row["TaxAmount"] = item.taxAmount;

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable ItemWisePurchaseReturnToDataTable(IEnumerable<ItemPurchaseReturnReportDtoR> items)
        {
            var table = new DataTable();

            // Define columns
            table.Columns.Add("SrNo", typeof(int));
            table.Columns.Add("ProductCode", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("PurchaseReturnID", typeof(int));
            table.Columns.Add("PurchaseReturnDate", typeof(DateTime));
            table.Columns.Add("VendorName", typeof(string));
            table.Columns.Add("Quantity", typeof(decimal));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("Discount", typeof(decimal));
            table.Columns.Add("TaxAmount", typeof(decimal));

            // Fill rows
            int srNo = 1;
            foreach (var item in items)
            {
                var row = table.NewRow();
                row["SrNo"] = srNo++;
                row["ProductCode"] = item.productCode;
                row["ProductName"] = item.productName;
                row["PurchaseReturnID"] = item.purchaseReturnID;
                row["PurchaseReturnDate"] = item.purchaseReturnDate;
                row["VendorName"] = item.vendorName;
                row["Quantity"] = item.quantity;
                row["UnitPrice"] = item.unitPrice;
                row["Discount"] = item.discount;
                row["TaxAmount"] = item.taxAmount;

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable ItemWiseSalesToDataTable(IEnumerable<ItemSalesReportDtoR> items)
        {
            var table = new DataTable();

            // Define columns
            table.Columns.Add("SrNo", typeof(int));
            table.Columns.Add("ProductCode", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("SalesID", typeof(int));
            table.Columns.Add("SalesDate", typeof(DateTime));
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("Quantity", typeof(decimal));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("Discount", typeof(decimal));
            table.Columns.Add("TaxAmount", typeof(decimal));

            // Fill rows
            int srNo = 1;
            foreach (var item in items)
            {
                var row = table.NewRow();
                row["SrNo"] = srNo++;
                row["ProductCode"] = item.productCode;
                row["ProductName"] = item.productName;
                row["SalesID"] = item.salesID;
                row["SalesDate"] = item.salesDate;
                row["CustomerName"] = item.customerName;
                row["Quantity"] = item.quantity;
                row["UnitPrice"] = item.unitPrice;
                row["Discount"] = item.discount;
                row["TaxAmount"] = item.taxAmount;

                table.Rows.Add(row);
            }

            return table;
        }
        public static DataTable ItemWiseSalesReturnToDataTable(IEnumerable<ItemSalesReturnReportDtoR> items)
        {
            var table = new DataTable();

            // Define columns
            table.Columns.Add("SrNo", typeof(int));
            table.Columns.Add("ProductCode", typeof(int));
            table.Columns.Add("ProductName", typeof(string));
            table.Columns.Add("SalesReturnID", typeof(int));
            table.Columns.Add("SalesReturnDate", typeof(DateTime));
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("Quantity", typeof(decimal));
            table.Columns.Add("UnitPrice", typeof(decimal));
            table.Columns.Add("Discount", typeof(decimal));
            table.Columns.Add("TaxAmount", typeof(decimal));

            // Fill rows
            int srNo = 1;
            foreach (var item in items)
            {
                var row = table.NewRow();
                row["SrNo"] = srNo++;
                row["ProductCode"] = item.productCode;
                row["ProductName"] = item.productName;
                row["SalesReturnID"] = item.salesReturnID;
                row["SalesReturnDate"] = item.salesReturnDate;
                row["CustomerName"] = item.customerName;
                row["Quantity"] = item.quantity;
                row["UnitPrice"] = item.unitPrice;
                row["Discount"] = item.discount;
                row["TaxAmount"] = item.taxAmount;

                table.Rows.Add(row);
            }

            return table;
        }
        public static async Task<byte[]> DownloadFileAsBytesStreamAsync(string url)
        {
            try
            {
                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync();
                using var ms = new MemoryStream();

                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
            catch (Exception exx)
            { 
            
            return Array.Empty<byte>(); 
            }
        }
        public static async Task <DataTable> CompanyToDataTable(CompanyDto company)
        {

                var table = new DataTable("DataSet1");

            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Address", typeof(string));
            table.Columns.Add("Phone", typeof(string));
            table.Columns.Add("img", typeof(byte[]));

            var row = table.NewRow();
            row["Name"] = company.Name;
            row["Address"] = company.Address;
            row["Phone"] = company.ContactPhone ?? "";
            
            if (!string.IsNullOrWhiteSpace(company.logoPath))
            {
                row["img"] = await DownloadFileAsBytesStreamAsync(company.logoPath);
            }
            else
            {
                row["img"] = DBNull.Value;
            }
            table.Rows.Add(row);

            return table;
        }
 
        public static DataTable ToPurchaseDataTable(List<PurchaseMasterDto> list)
        {
            var table = new DataTable("DataSet2");

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("VendorName", typeof(string));
            table.Columns.Add("SubTotal", typeof(double));
            table.Columns.Add("TotalDiscount", typeof(double));
            table.Columns.Add("TaxAmount", typeof(double));
            table.Columns.Add("Total", typeof(double));

            foreach (var item in list)
            {
                var row = table.NewRow();
                row["Id"] = item.Id;
                row["Date"] = item.Date;
                row["VendorName"] = item.VendorName;
                row["SubTotal"] = item.SubTotal;
                row["TotalDiscount"] = item.TotalDiscount;
                row["TaxAmount"] = item.TaxAmount;
                row["Total"] = item.Total;
                table.Rows.Add(row);
            }

            return table;
        }
        
        public static DataTable ToPurchaseReturnDataTable(List<PurchaseReturnMasterDto> list)
        {
            var table = new DataTable("DataSet2");

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("VendorName", typeof(string));
            table.Columns.Add("SubTotal", typeof(double));
            table.Columns.Add("TotalDiscount", typeof(double));
            table.Columns.Add("TaxAmount", typeof(double));
            table.Columns.Add("Total", typeof(double));

            foreach (var item in list)
            {
                var row = table.NewRow();
                row["Id"] = item.Id;
                row["Date"] = item.Date;
                row["VendorName"] = item.VendorName ?? string.Empty;
                row["SubTotal"] = Convert.ToDouble(item.SubTotal);
                row["TotalDiscount"] = Convert.ToDouble(item.TotalDiscount);
                row["TaxAmount"] = Convert.ToDouble(item.TaxAmount);
                row["Total"] = Convert.ToDouble(item.Total);
                table.Rows.Add(row);
            }

            return table;
        }

        public static DataTable ToSalesDataTable(List<SalesMasterDto> list)
        {
            var table = new DataTable("DataSet2");

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("SubTotal", typeof(double));
            table.Columns.Add("TotalDiscount", typeof(double));
            table.Columns.Add("TaxAmount", typeof(double));
            table.Columns.Add("BalanceAmount", typeof(double));

            foreach (var item in list)
            {
                var row = table.NewRow();
                row["Id"] = item.Id;
                row["Date"] = item.Date;
                row["CustomerName"] = item.CustomerName;
                row["SubTotal"] = item.SubTotal;
                row["TotalDiscount"] = item.TotalDiscount;
                row["TaxAmount"] = item.TaxAmount;
                row["BalanceAmount"] = item.BalanceAmount;
                table.Rows.Add(row);
            }

            return table;
        }

        public static DataTable ToSalesReturnDataTable(List<SalesReturnMasterDto> list)
        {
            var table = new DataTable("DataSet2");

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("CustomerName", typeof(string));
            table.Columns.Add("SubTotal", typeof(double));
            table.Columns.Add("TotalDiscount", typeof(double));
            table.Columns.Add("TaxAmount", typeof(double));
            table.Columns.Add("BalanceAmount", typeof(double));

            foreach (var item in list)
            {
                var row = table.NewRow();
                row["Id"] = item.Id;
                row["Date"] = item.Date;
                row["CustomerName"] = item.CustomerName;
                row["SubTotal"] = item.SubTotal;
                row["TotalDiscount"] = item.TotalDiscount;
                row["TaxAmount"] = item.TaxAmount;
                row["BalanceAmount"] = item.BalanceAmount;
                table.Rows.Add(row);
            }

            return table;
        }

    }
}

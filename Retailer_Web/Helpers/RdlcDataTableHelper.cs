using QRCoder;
using Retailer.POS.Web.ApiDTOs;
using Retailer.Web.Models;
using System.Data;
    using System.Drawing.Imaging;

    namespace Retailer.Web.Helpers
    {
        public class RdlcDataTableHelper
        {

            public byte[]? GetImageBytesFromPath(IWebHostEnvironment _env ,string relativePath)
            {
                if (string.IsNullOrEmpty(relativePath))
                    return null;

                // Remove leading slashes
                relativePath = relativePath.TrimStart('/');

                // Remove prefix if present
                var prefix = "uploads/CompanyLogo/";
                if (relativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = relativePath.Substring(prefix.Length);
                }

                // Build physical path
                var folderPath = Path.Combine(_env.WebRootPath, "uploads", "CompanyLogo");
                var filePath = Path.Combine(folderPath, relativePath);

                if (!System.IO.File.Exists(filePath))
                    return null; // safer than throw for images

                return System.IO.File.ReadAllBytes(filePath);
            }

            public string getFullpathCompanyLogo(IWebHostEnvironment _env, string relativePath)
            {
                if (string.IsNullOrEmpty(relativePath))
                    return null;

                // Remove leading slashes
                relativePath = relativePath.TrimStart('/');

                // Remove "uploads/CompanyLogo/" prefix if present
                var prefix = "uploads/CompanyLogo/";
                if (relativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = relativePath.Substring(prefix.Length);
                }

                // Combine with wwwroot/uploads/CompanyLogo
                var folderPath = Path.Combine(_env.WebRootPath, "uploads", "CompanyLogo");
                return   Path.Combine(folderPath, relativePath);

            }
       
                public IFormFile? GetFormFileFromPath(IWebHostEnvironment _env, string relativePath )
            {
                if (string.IsNullOrEmpty(relativePath))
                    return null;

                // Remove leading slashes
                relativePath = relativePath.TrimStart('/');

                // Remove "uploads/CompanyLogo/" prefix if present
                var prefix = "uploads/CompanyLogo/";
                if (relativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = relativePath.Substring(prefix.Length);
                }

                // Combine with wwwroot/uploads/CompanyLogo
                var folderPath = Path.Combine(_env.WebRootPath, "uploads", "CompanyLogo");
                var filePath = Path.Combine(folderPath, relativePath);

                if (!System.IO.File.Exists(filePath))
                    throw new FileNotFoundException("File not found", filePath);

                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                IFormFile formFile = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "application/octet-stream" // or detect MIME type dynamically
                };

                return formFile;
            }
            // ===============================
            // DataSet1 — Company / Header
            // ===============================
            public DataTable CompanyToDataTable(CompanyViewModel model, object imgValue)
            {
                var dt = new DataTable("DataSet1"); 
            dt.Columns.Add("Name");
                dt.Columns.Add("Address");
                dt.Columns.Add("ContactEmail");
                dt.Columns.Add("ContactPhone");
                dt.Columns.Add("CNIC");
                dt.Columns.Add("Province");
                dt.Columns.Add("City");
                dt.Columns.Add("img", typeof(byte[]));
                try
                {


                    dt.Rows.Add(
                        model.Name,
                        model.Address,
                        model.ContactEmail,
                        model.ContactPhone,
                        model.CNIC,
                        model.Province,
                        model.Address,
                        imgValue

                    );


                }
                catch (Exception exx) { 
                }
                return dt;

            }
            public byte[] GenerateQrCode(string text)
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCode(qrData);
                using var bitmap = qrCode.GetGraphic(20);

                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
            // ===============================
            // DataSet2 — Invoice Summary
            // ===============================
            public DataTable InvoiceSummaryToDataTable(SalesMasterDto model)
            {
                var dt = new DataTable("DataSet2");

                dt.Columns.Add("Date", typeof(DateTime));
                dt.Columns.Add("CustomerName");
                dt.Columns.Add("SubTotal", typeof(decimal));
                dt.Columns.Add("TotalDiscount", typeof(decimal));
                dt.Columns.Add("TaxAmount", typeof(decimal));
                dt.Columns.Add("BalanceAmount", typeof(decimal));
                dt.Columns.Add("City");
                dt.Columns.Add("Province");
                dt.Columns.Add("CNIC");
                dt.Columns.Add("img", typeof(byte[]));

                dt.Rows.Add(
                    model.Date,
                    model.CustomerName,
                    model.SubTotal,
                    model.TotalDiscount,
                    model.TaxAmount,
                    model.BalanceAmount,
                    "model.City",
                    "model.Province",
                    "model.CNIC"
                    , GenerateQrCode(model.Id.ToString())
                ) ;

                return dt;
            }

            // ===============================
            // DataSet3 — Invoice Items
            // ===============================
            public DataTable InvoiceItemsToDataTable(IEnumerable<SalesDetailDto> items)
            {
                var dt = new DataTable("DataSet3");

                dt.Columns.Add("ItemCode");
                dt.Columns.Add("ItemName");
                dt.Columns.Add("Rate", typeof(decimal));
                dt.Columns.Add("Qty", typeof(int));
                dt.Columns.Add("Discount", typeof(decimal));
                dt.Columns.Add("TaxPercentage", typeof(decimal));
                dt.Columns.Add("TaxAmount", typeof(decimal));
                dt.Columns.Add("Amount", typeof(decimal));
                dt.Columns.Add("extraTax", typeof(decimal));
                dt.Columns.Add("furtherTax", typeof(decimal));
                dt.Columns.Add("hsCode");
                dt.Columns.Add("saleType");
                dt.Columns.Add("UOM");
                dt.Columns.Add("furtherTaxP", typeof(decimal));
                dt.Columns.Add("extraTaxP", typeof(decimal));

                foreach (var i in items)
                {
                    dt.Rows.Add(
                        i.ItemCode,
                        i.ItemName,
                        i.Rate,
                        i.Qty,
                        i.Discount,
                        i.TaxPercentage,
                        i.TaxAmount,
                        i.Amount,
                        i.extraTax,
                        i.furtherTax,
                        i.HsCode,
                        i.SaleType,
                        i.UOM,
                        i.furtherTaxP,
                        i.extraTaxP
                    );
                }

                return dt;
            }
        }
    }
 
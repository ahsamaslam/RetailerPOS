namespace Retailer.Web.ApiDTOs
{
    public class SaleInvoiceSettingDto
    {
        public int Id { get; set; }

        // Page Setup
        public string? PageSize { get; set; }          // A4, A5, Letter, Custom
        public string? Orientation { get; set; }       // Portrait / Landscape
        public decimal? PageWidthMM { get; set; }      // Used when PageSize = 'Custom'
        public decimal? PageHeightMM { get; set; }

        // Header Section 
        public string? Header1Label { get; set; }
        public string? Header1Align { get; set; }
        public string? Header1Value { get; set; }
        public int? Header1FontSize { get; set; }
        public string? Header1Margin { get; set; }
        public string? Header1Padding { get; set; }

        public string? Header2Align { get; set; }
        public string? Header2Text { get; set; }
        public int? Header2FontSize { get; set; }
        public string? Header2Margin { get; set; } 
        public string? Header2Padding { get; set; }

        public string? Header3Align { get; set; }
        public string? Header3Text { get; set; }
        public int? Header3FontSize { get; set; }
        public string? Header3Margin { get; set; }
        public string? Header3Padding { get; set; }

        // GST Section
        public bool ShowGST { get; set; }

        // Column Labels & Font Sizes
        public string? SrAlign { get; set; }
        public string? SrLabel { get; set; }
        public int? SrFontSize { get; set; }

        public string? NameAlign { get; set; }
        public string? NameLabel { get; set; }
        public int? NameFontSize { get; set; }

        public int? RateRound { get; set; }
        public string? RateAlign { get; set; }
        public string? RateLabel { get; set; }
        public int? RateFontSize { get; set; }

        public string? QtyAlign { get; set; }
        public string? QtyLabel { get; set; }
        public int? QtyRound { get; set; }
        public int? QtyFontSize { get; set; }

        public int? DiscountRound { get; set; }
        public string? DiscountAlign { get; set; }
        public string? DiscountLabel { get; set; }
        public int? DiscountFontSize { get; set; }

        public int? TotalRound { get; set; }
        public string? TotalAlign { get; set; }
        public string? TotalLabel { get; set; }
        public int? TotalFontSize { get; set; }

        public string? GSTAlign { get; set; }
        public int? GSTRound { get; set; }
        public string? GSTLabel { get; set; }
        public int? GSTFontSize { get; set; }

        public int? GSTPercentRound { get; set; }
        public string? GSTPercentAlign { get; set; }
        public string? GSTPercentLabel { get; set; }
        public int? GSTPercentFontSize { get; set; }

        // Footer Section
        public string? Footer1Align { get; set; }
        public string? Footer1Text { get; set; }
        public int? Footer1FontSize { get; set; }
        public string? Footer1Margin { get; set; }
        public string? Footer1Padding { get; set; }

        public string? Footer2Align { get; set; }
        public string? Footer2Text { get; set; }
        public int? Footer2FontSize { get; set; }
        public string? Footer2Margin { get; set; }
        public string? Footer2Padding { get; set; }

        public string? Footer3Align { get; set; }
        public string? Footer3Text { get; set; }
        public int? Footer3FontSize { get; set; }
        public string? Footer3Margin { get; set; }
        public string? Footer3Padding { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid companyID { get; set; }
        public int branchID { get; set; }
        public int ShowLogo { get; set; }
        public string? LogoCss { get; set; }
        
    }
}

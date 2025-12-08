using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace Retailer.Api.DTOs
{
    public class CompanyDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string? CNIC { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string NTN { get; set; } = string.Empty;           
        public string STRN { get; set; } = string.Empty;           
        public string logoPath { get; set; } = string.Empty;
        public bool fbrActive { get; set; } = false; 
        public string? Province { get; set; }
        public string? pralToken { get; set; }
        public string? fbrToken { get; set; } 
        public int invoiceCounter { get; set; } = 0;
        public int invoicePerPage { get; set; } = 0;
    }
}

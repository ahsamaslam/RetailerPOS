using Retailer.POS.Api.Entities;

namespace Retailer.Api.Entities
{
    public class Cities 
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ProvienceId { get; set; }
        public Provience Provience { get; set; } = new Provience();
    }
}

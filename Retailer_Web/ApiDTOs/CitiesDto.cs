namespace Retailer.Web.ApiDTOs
{
    public class CitiesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProvienceId { get; set; }
        public ProvienceDto Provience { get; set; }
    }
}

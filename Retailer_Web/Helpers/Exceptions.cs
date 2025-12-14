namespace Retailer.Web.Helpers
{
    public class ApiUnauthorizedException : Exception
    {
        public ApiUnauthorizedException(string message = "API returned 401 Unauthorized") : base(message) { }
    }
}

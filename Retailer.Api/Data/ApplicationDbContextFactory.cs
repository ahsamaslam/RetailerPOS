using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Retailer.POS.Api.Data;

namespace Retailer.API.Data
{
    public class ApplicationDbContextFactory
         : IDesignTimeDbContextFactory<RetailerDbContext>
    {
        public RetailerDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string not found for migrations.");

            var optionsBuilder = new DbContextOptionsBuilder<RetailerDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new RetailerDbContext(optionsBuilder.Options);
        }
    }
}

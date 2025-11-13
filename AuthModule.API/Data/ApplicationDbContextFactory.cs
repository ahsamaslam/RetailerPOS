using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthModule.API.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var connStr = Environment.GetEnvironmentVariable("AUTHMODULE_CONN")
              ?? "Server=localhost;Database=AuthModuleDB;User Id=sa;Password=sa;Trusted_Connection=False;TrustServerCertificate=true;";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // TODO: Replace with your actual connection string or read from env variable
            optionsBuilder.UseSqlServer(connStr);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

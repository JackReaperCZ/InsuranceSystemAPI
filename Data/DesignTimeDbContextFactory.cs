using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InsuranceSystemAPI.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<InsuranceDbContext>
    {
        public InsuranceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<InsuranceDbContext>();
            
            // Use MariaDB for design-time operations
            optionsBuilder.UseMySql(
                "Server=localhost;Database=InsuranceSystemDB;User=root;Password=;Port=3306;",
                new MySqlServerVersion(new Version(10, 5, 0)) // MariaDB 10.5
            );

            return new InsuranceDbContext(optionsBuilder.Options);
        }
    }
}
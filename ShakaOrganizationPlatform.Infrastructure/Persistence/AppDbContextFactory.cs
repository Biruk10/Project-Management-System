using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShakaOrganizationPlatform.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // ለ Design-time (Migration) ጊዜ የሚያገለግል የ connection string
        optionsBuilder.UseNpgsql("Host=localhost;Database=ShakaOrgDb;Username=postgres;Password=psql");

        return new AppDbContext(optionsBuilder.Options);
    }
}
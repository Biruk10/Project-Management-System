using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Name).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Email).IsRequired().HasMaxLength(200);
        builder.Property(o => o.TimeZone).HasMaxLength(100).HasDefaultValue("UTC");
        builder.Property(o => o.Phone).HasMaxLength(50);
        builder.Property(o => o.Address).HasMaxLength(500);
        builder.Property(o => o.LogoUrl).HasMaxLength(500);
        builder.HasIndex(o => o.Email).IsUnique();
    }
}

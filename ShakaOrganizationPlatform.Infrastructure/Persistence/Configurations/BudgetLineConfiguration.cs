using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Infrastructure.Persistence.Configurations;

public class BudgetLineConfiguration : IEntityTypeConfiguration<BudgetLine>
{
    public void Configure(EntityTypeBuilder<BudgetLine> builder)
    {
        builder.ToTable("BudgetLines");
        builder.HasKey(bl => bl.Id);
        builder.Property(bl => bl.Category).IsRequired().HasMaxLength(200);
        builder.Property(bl => bl.Description).HasMaxLength(500);
        builder.Property(bl => bl.AllocatedAmount).HasPrecision(18, 2);
    }
}

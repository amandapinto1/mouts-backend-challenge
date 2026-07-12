using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Mapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        AuditableEntityConfiguration.Configure(builder);

        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
        builder.Property(s => s.SaleDate).IsRequired();
        builder.Property(s => s.CustomerId).IsRequired();
        builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(150);
        builder.Property(s => s.BranchId).IsRequired();
        builder.Property(s => s.BranchName).IsRequired().HasMaxLength(150);
        builder.Property(s => s.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(s => s.IsCancelled).IsRequired().HasDefaultValue(false);

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.SaleNumber).IsUnique();
        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.BranchId);
        builder.HasIndex(s => s.SaleDate);
    }
}

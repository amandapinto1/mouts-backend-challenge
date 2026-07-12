using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Mapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        BaseEntityConfiguration.Configure(builder);

        builder.Property(i => i.SaleId).IsRequired();
        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(i => i.Discount).IsRequired().HasColumnType("decimal(5,2)");
        builder.Property(i => i.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(i => i.IsCancelled).IsRequired().HasDefaultValue(false);

        builder.HasIndex(i => i.SaleId);
        builder.HasIndex(i => i.ProductId);
    }
}

using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Mapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        AuditableEntityConfiguration.Configure(builder);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.Description).HasMaxLength(2000);
        builder.Property(p => p.CategoryId).IsRequired();
        builder.Property(p => p.Image).HasMaxLength(500);
        builder.Property(p => p.RatingRate).HasColumnType("decimal(3,1)");
        builder.Property(p => p.RatingCount);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Title);
    }
}

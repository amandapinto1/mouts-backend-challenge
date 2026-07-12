using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Mapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");

        AuditableEntityConfiguration.Configure(builder);

        builder.Property(b => b.Name).IsRequired().HasMaxLength(150);
        builder.Property(b => b.Address).HasMaxLength(300);
        builder.Property(b => b.City).HasMaxLength(100);
        builder.Property(b => b.State).HasMaxLength(100);
        builder.Property(b => b.IsActive).IsRequired().HasDefaultValue(true);
    }
}

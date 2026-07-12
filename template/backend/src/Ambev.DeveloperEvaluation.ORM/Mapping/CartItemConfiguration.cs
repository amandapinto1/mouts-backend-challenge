using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM.Mapping.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        BaseEntityConfiguration.Configure(builder);

        builder.Property(i => i.CartId).IsRequired();
        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.Quantity).IsRequired();
    }
}

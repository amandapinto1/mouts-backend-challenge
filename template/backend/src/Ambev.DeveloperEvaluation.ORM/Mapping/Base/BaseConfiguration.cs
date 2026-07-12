using Ambev.DeveloperEvaluation.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping.Base
{
    public static class BaseEntityConfiguration
    {
        public static void Configure<TEntity>(EntityTypeBuilder<TEntity> builder)
            where TEntity : BaseEntity
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");
        }
    }

    public static class AuditableEntityConfiguration
    {
        public static void Configure<TEntity>(EntityTypeBuilder<TEntity> builder)
            where TEntity : AuditableEntity
        {
            BaseEntityConfiguration.Configure(builder);
            builder.Property(x => x.CreatedBy).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedBy);
            builder.Property(x => x.UpdatedAt);
        }
    }
}

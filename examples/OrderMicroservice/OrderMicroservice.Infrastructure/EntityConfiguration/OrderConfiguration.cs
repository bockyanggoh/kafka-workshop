using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.Infrastructure.EntityConfiguration
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.ToTable("tbl_orders");
            builder.HasKey(i => i.OrderId);
            builder.HasIndex(i => i.Username);

            builder.Property(i => i.Username).HasColumnName("username").HasColumnType("varchar(64)");
            builder.Property(i => i.CreatedTs).HasColumnName("created_ts").HasDefaultValueSql("GetDate()");
            builder.Property(i => i.OrderId).HasColumnName("order_id").HasColumnType("varchar(64)");
        }
    }
}
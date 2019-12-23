using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.Infrastructure.EntityConfiguration
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemEntity>
    {
        public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
        {
            builder.ToTable("tbl_order_items");
            builder.HasKey(i => i.OrderItemId);
            builder.HasIndex(i => i.OrderId);
            builder.HasIndex(i => i.ItemId);

            builder.HasOne(i => i.OrderEntity)
                .WithMany(i => i.OrderItems)
                .IsRequired().HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
            
            builder.Property(i => i.OrderId).HasColumnName("order_id").IsRequired();
            builder.Property(i => i.OrderItemId).HasColumnName("order_item_id").IsRequired();
            builder.Property(i => i.ItemId).HasColumnName("item_id").IsRequired();
        }
    }
}
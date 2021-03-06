using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderMicroservice.Domain.AggregateModel;

namespace OrderMicroservice.Infrastructure.EntityConfiguration
{
    public class ItemConfiguration : IEntityTypeConfiguration<ItemEntity>
    {
        public void Configure(EntityTypeBuilder<ItemEntity> builder)
        {
            builder.ToTable("tbl_items");
            builder.HasKey(i => i.ItemId);
            builder.HasIndex(i => i.ItemName)
                .IsUnique();
            builder.HasIndex(i => i.ItemId);

            builder.Property(i => i.CostPrice)
                .HasColumnName("cost_price");
            builder.Property(i => i.ItemId)
                .HasColumnName("item_id")
                .HasColumnType("varchar(64)");
            builder.Property(i => i.ItemType)
                .HasColumnName("item_type")
                .HasColumnType("varchar(50)");
            builder.Property(i => i.ItemName)
                .HasColumnName("item_name")
                .HasColumnType("varchar(100)");
            builder.Property(i => i.DateCreated).IsRequired().HasColumnName("created_ts")
                .HasDefaultValueSql("GetDate()");
            
        }
    }
}
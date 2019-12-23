using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentMicroservice.Domain.AggregateModel;

namespace PaymentMicroservice.Infrastructure.EntityConfiguration
{
    public class PaymentConfiguration: IEntityTypeConfiguration<PaymentEntity>
    {
        public void Configure(EntityTypeBuilder<PaymentEntity> builder)
        {
            builder.ToTable("tbl_payments");
            builder.HasKey(i => i.PaymentId);
            builder.HasIndex(i => i.PaymentId).IsUnique();
            builder.HasIndex(i => i.OrderId).IsUnique();
            builder.Property(i => i.PaymentId)
                .HasColumnName("payment_id")
                .HasColumnType("varchar(64)");
            builder.Property(i => i.OrderId).IsRequired()
                .HasColumnName("order_id")
                .HasColumnType("varchar(64)");
            builder.Property(i => i.Username).IsRequired()
                .HasColumnName("username")
                .HasColumnType("varchar(50)");
            builder.Property(i => i.PaymentStatus).IsRequired()
                .HasColumnName("payment_status")
                .HasColumnType("varchar(20)");
            builder.Property(i => i.AmountPayable).IsRequired()
                .HasColumnName("amount_payable");
            builder.Property(i => i.CreatedTs).IsRequired().HasColumnName("created_ts");
        }
    }
}
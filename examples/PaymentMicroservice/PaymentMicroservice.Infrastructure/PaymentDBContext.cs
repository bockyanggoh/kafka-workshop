using Microsoft.EntityFrameworkCore;
using PaymentMicroservice.Domain.AggregateModel;
using PaymentMicroservice.Infrastructure.EntityConfiguration;

namespace PaymentMicroservice.Infrastructure
{
    public class PaymentDBContext: DbContext
    {
        public DbSet<PaymentEntity> Payments { get; set; }

        public PaymentDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        }
    }
}
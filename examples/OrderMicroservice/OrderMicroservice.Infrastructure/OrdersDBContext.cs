using Microsoft.EntityFrameworkCore;
using OrderMicroservice.Domain.AggregateModel;
using OrderMicroservice.Infrastructure.EntityConfiguration;

namespace OrderMicroservice.Infrastructure
{
    public class OrdersDBContext : DbContext
    {
        private const string DB_SCHEMA = "order-schema";
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }

        public OrdersDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
        }
    }
}
﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderMicroservice.Infrastructure;

namespace OrderMicroservice.Migrations
{
    [DbContext(typeof(OrdersDBContext))]
    partial class OrdersDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OrderMicroservice.Domain.AggregateModel.ItemEntity", b =>
                {
                    b.Property<string>("ItemId")
                        .HasColumnName("item_id")
                        .HasColumnType("varchar(64)");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("created_ts")
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GetDate()");

                    b.Property<string>("ItemName")
                        .HasColumnName("item_name")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ItemType")
                        .IsRequired()
                        .HasColumnName("item_type")
                        .HasColumnType("varchar(50)");

                    b.HasKey("ItemId");

                    b.HasIndex("ItemId");

                    b.HasIndex("ItemName")
                        .IsUnique()
                        .HasFilter("[item_name] IS NOT NULL");

                    b.ToTable("tbl_items");
                });
#pragma warning restore 612, 618
        }
    }
}

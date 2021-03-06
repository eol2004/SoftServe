﻿// <auto-generated />
using System;
using GameStore.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GameStore.Persistence.Migrations
{
    [DbContext(typeof(GameStoreDbContext))]
    partial class GameStoreDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("GameStore.Domain.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDateTime");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("GameStore.Domain.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDateTime");

                    b.Property<string>("Description")
                        .HasMaxLength(4096);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<decimal>("Price");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GameStore.Domain.Models.GameCategory", b =>
                {
                    b.Property<int>("GameId");

                    b.Property<int>("CategoryId");

                    b.HasKey("GameId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("GameCategories");
                });

            modelBuilder.Entity("GameStore.Domain.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("AddedDateTime");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<bool>("GiftWrap");

                    b.Property<string>("Line1")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Line2")
                        .HasMaxLength(128);

                    b.Property<string>("Line3")
                        .HasMaxLength(128);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("State")
                        .HasMaxLength(128);

                    b.Property<string>("Zip")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("GameStore.Domain.Models.OrderLine", b =>
                {
                    b.Property<int>("GameId");

                    b.Property<int>("OrderId");

                    b.Property<int>("Quantity");

                    b.HasKey("GameId", "OrderId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderLines");
                });

            modelBuilder.Entity("GameStore.Domain.Models.GameCategory", b =>
                {
                    b.HasOne("GameStore.Domain.Models.Category", "Category")
                        .WithMany("GameCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GameStore.Domain.Models.Game", "Game")
                        .WithMany("GameCategories")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GameStore.Domain.Models.OrderLine", b =>
                {
                    b.HasOne("GameStore.Domain.Models.Game", "Game")
                        .WithMany("Lines")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GameStore.Domain.Models.Order", "Order")
                        .WithMany("Lines")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

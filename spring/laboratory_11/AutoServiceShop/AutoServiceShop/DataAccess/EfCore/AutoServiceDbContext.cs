using AutoServiceShop.DataAccess.EfCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceShop.DataAccess.EfCore
{
    public class AutoServiceDbContext : DbContext
    {
        public AutoServiceDbContext(DbContextOptions<AutoServiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<EfProduct> Products => Set<EfProduct>();
        public DbSet<EfCategory> Categories => Set<EfCategory>();
        public DbSet<EfOrder> Orders => Set<EfOrder>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Categories: 1 -> many Products
            modelBuilder.Entity<EfCategory>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.Name).HasColumnName("Name").IsRequired();
                entity.Property(x => x.Description).HasColumnName("Description");
            });

            modelBuilder.Entity<EfProduct>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.ShortName).HasColumnName("ShortName").IsRequired();
                entity.Property(x => x.FullName).HasColumnName("FullName");
                entity.Property(x => x.Description).HasColumnName("Description");

                entity.Property(x => x.CategoryId).HasColumnName("CategoryId");
                entity.HasOne(x => x.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.Price).HasColumnName("Price");
                entity.Property(x => x.Quantity).HasColumnName("Quantity");
                entity.Property(x => x.Rating).HasColumnName("Rating");
                entity.Property(x => x.Country).HasColumnName("Country");
                entity.Property(x => x.Discount).HasColumnName("Discount");

                // SQLite часто хранит bool как INTEGER 0/1
                entity.Property(x => x.InStock).HasColumnName("InStock");

                entity.Property(x => x.Manufacturer).HasColumnName("Manufacturer");
                entity.Property(x => x.Color).HasColumnName("Color");
                entity.Property(x => x.Size).HasColumnName("Size");
                entity.Property(x => x.ImagePaths).HasColumnName("ImagePaths");
            });

            modelBuilder.Entity<EfOrder>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).HasColumnName("Id");
                entity.Property(x => x.ProductId).HasColumnName("ProductId");

                entity.HasOne(x => x.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.Quantity).HasColumnName("Quantity");
                entity.Property(x => x.OrderDate).HasColumnName("OrderDate");
                entity.Property(x => x.CustomerName).HasColumnName("CustomerName").IsRequired();
            });
        }
    }
}


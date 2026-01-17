using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LentzCraftServices.Domain.Entities;

namespace LentzCraftServices.Infrastructure.Data;

/// <summary>
/// Main application database context.
/// Extends IdentityDbContext to support ASP.NET Core Identity for authentication.
/// </summary>
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Product entity
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Category).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.IsPublic).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();

            // Performance indexes
            entity.HasIndex(e => e.IsPublic);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasIndex(e => new { e.IsPublic, e.Category });
            entity.HasIndex(e => new { e.IsPublic, e.Status });

            // Configure one-to-many relationship with ProductImage
            entity.HasMany(e => e.Images)
                  .WithOne(e => e.Product)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ProductImage entity
        builder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IsPrimary).IsRequired();
            entity.Property(e => e.ProductId).IsRequired();

            // Index for faster lookups
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.ProductId, e.IsPrimary });
        });
    }
}


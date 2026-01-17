using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using LentzCraftServices.Domain.Entities;
using LentzCraftServices.Domain.Enums;

namespace LentzCraftServices.Infrastructure.Data;

/// <summary>
/// Handles database initialization and seeding.
/// Creates admin user and sample product data for development.
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(
        ApplicationDbContext context, 
        UserManager<IdentityUser> userManager,
        IConfiguration configuration,
        ILogger? logger = null)
    {
        // Check if seeding is enabled (default: only in Development)
        var enableSeedingValue = configuration["Database:EnableSeeding"];
        var enableSeeding = string.IsNullOrEmpty(enableSeedingValue) 
            ? string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase)
            : bool.TryParse(enableSeedingValue, out var result) && result;

        if (!enableSeeding)
        {
            logger?.LogInformation("Database seeding is disabled. Skipping initialization.");
            return;
        }

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if database has been seeded
        if (await context.Products.AnyAsync())
        {
            logger?.LogInformation("Database already contains products. Skipping seeding.");
            return; // Database already seeded
        }

        // Create admin user from configuration or environment variables
        var adminEmail = configuration["Admin:Email"] 
            ?? Environment.GetEnvironmentVariable("ADMIN_EMAIL") 
            ?? "admin@lentzcrafts.com";
        var adminPassword = configuration["Admin:Password"] 
            ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD") 
            ?? "Admin@123"; // Fallback for development only

        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            await userManager.CreateAsync(adminUser, adminPassword);
        }

        // Seed sample products
        var products = new List<Product>
        {
            new Product
            {
                Name = "Ripple Blanket",
                Description = "Handmade crochet ripple blanket in soft, warm yarn. Perfect for snuggling on cold evenings. Beautiful ripple stitch pattern.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 3,
                Price = 30.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-20),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/RippleBlanket.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Baby Booties 2",
                Description = "Adorable handcrafted baby booties. Made with soft, comfortable yarn perfect for little feet.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 20.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-19),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyBooties2.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Animal Hats",
                Description = "Adorable crochet animal hats. Perfect gift for children or collectors. Made with soft, durable yarn.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 2,
                Price = 17.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-5),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/Animalhats.jpg", IsPrimary = true }
                }
            },
            // Additional crochet products
            new Product
            {
                Name = "Baby Blanket",
                Description = "Beautiful handcrafted baby blanket. Made with care and attention to detail using soft, baby-safe yarn.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 22.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-4),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyBlanket.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Baby Booties",
                Description = "Adorable handcrafted baby booties. Made with soft, comfortable yarn perfect for little feet.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 25.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-3),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyBooties.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Baby Booties 3",
                Description = "Adorable handcrafted baby booties. Made with soft, comfortable yarn perfect for little feet.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 27.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyBooties3.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Baby Booties 4",
                Description = "Adorable handcrafted baby booties. Made with soft, comfortable yarn perfect for little feet.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 30.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyBooties4.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Baby Booties 5",
                Description = "Adorable handcrafted baby booties. Made with soft, comfortable yarn perfect for little feet.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 32.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyBooties5.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Baby Booties 6",
                Description = "Adorable handcrafted baby booties. Made with soft, comfortable yarn perfect for little feet.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 35.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyBooties6.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Baby Flip Flops",
                Description = "Cute handcrafted baby flip flops. Made with soft, comfortable yarn perfect for little feet.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 37.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyFlipFlops.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Baby Flip Flops 2",
                Description = "Cute handcrafted baby flip flops. Made with soft, comfortable yarn perfect for little feet.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 40.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/BabyFlipFlops2.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Boot Cuffs 1",
                Description = "Stylish handcrafted boot cuffs. Perfect for keeping warm and adding a fashionable touch to your boots.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 42.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/Bootcuffs1.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Boot Cuffs 2",
                Description = "Stylish handcrafted boot cuffs. Perfect for keeping warm and adding a fashionable touch to your boots.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 45.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/Bootcuffs2.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Boot Cuffs 3",
                Description = "Stylish handcrafted boot cuffs. Perfect for keeping warm and adding a fashionable touch to your boots.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 47.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/Bootcuffs3.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Chiefs Cup Cozy",
                Description = "Handcrafted cup cozy featuring Chiefs team colors. Perfect for keeping your drinks warm and showing team spirit.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 50.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/ChiefsCupCozy.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Cup Cozies",
                Description = "Handcrafted cup cozies. Perfect for keeping your drinks warm and adding a personal touch to your beverages.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 52.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/CupCozies.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Cup Cozies 2",
                Description = "Handcrafted cup cozies. Perfect for keeping your drinks warm and adding a personal touch to your beverages.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 55.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/CupCozies2.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Daisy Towel",
                Description = "Beautiful handcrafted daisy towel. Perfect for kitchen or bathroom use with a lovely daisy design.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 57.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/DaisyTowel.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Daisy Baby Booties",
                Description = "Adorable handcrafted daisy baby booties. Made with soft, comfortable yarn with a beautiful daisy design.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 30.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/DaisyBabyBooties.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Winter Towel",
                Description = "Beautiful handcrafted winter towel. Perfect for kitchen or bathroom use with a lovely winter design.",
                Category = ProductCategory.Crochet,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 62.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow,
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/crochet/WinterTowel.jpg", IsPrimary = true }
                }
            },
            // Woodworking items
            new Product
            {
                Name = "Best Mom Ever Box",
                Description = "Beautiful handcrafted wooden box with 'Best Mom Ever' engraving. Perfect gift for Mother's Day or any special occasion.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 75.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-18),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/BestMomEverBox.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Best Mom Ever Box 1",
                Description = "Beautiful handcrafted wooden box with 'Best Mom Ever' engraving. Perfect gift for Mother's Day or any special occasion.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 87.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-17),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/BestMomEverBox1.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Chiefs Box",
                Description = "Handcrafted wooden box featuring Kansas City Chiefs team design. Perfect for Chiefs fans and collectors.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 100.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-16),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/CheifsBox.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Christmas Tree with Lights",
                Description = "Beautiful wooden Christmas tree decoration with integrated lights. Perfect for holiday home decor.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 112.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-15),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/ChristmasTreeWLights.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Easy Tiger Box",
                Description = "Handcrafted wooden box with 'Easy Tiger' engraving. Unique design perfect for gift giving.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 125.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-14),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/EasyTigerBox.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Engraved Boxes",
                Description = "Set of beautifully engraved wooden boxes. Each box features unique designs and can be personalized.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 137.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-13),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/EngravedBoxes.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Flower Door",
                Description = "Handcrafted wooden door with beautiful flower design. Perfect for adding a decorative touch to any room.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 150.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-12),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/FlowerDoor.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Hanging Plant Box",
                Description = "Beautiful wooden hanging planter box. Perfect for displaying plants and adding natural beauty to your home.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 162.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-11),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/HangingPlantBox.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Hanging Plant Box 1",
                Description = "Beautiful wooden hanging planter box. Perfect for displaying plants and adding natural beauty to your home.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 175.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-10),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/HangingPlantBox1.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "In Bliss Box",
                Description = "Handcrafted wooden box with 'In Bliss' engraving. Perfect for storing keepsakes or as a thoughtful gift.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 187.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-9),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/InBlissBox.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Mom Boxes",
                Description = "Set of beautifully crafted wooden boxes perfect for Mother's Day or any occasion. Each box is handcrafted with care.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 200.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-8),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/MomBoxes.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Wine Bottle Holder",
                Description = "Elegant wooden wine bottle holder. Handcrafted to securely display your favorite wine bottles in style.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 212.50m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-7),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/WineBottleHolder.jpg", IsPrimary = true }
                }
            },
            new Product
            {
                Name = "Wine Bottle Holder 1",
                Description = "Elegant wooden wine bottle holder. Handcrafted to securely display your favorite wine bottles in style.",
                Category = ProductCategory.Woodworking,
                Status = ProductStatus.Available,
                Quantity = 1,
                Price = 225.00m,
                IsPublic = true,
                CreatedDate = DateTime.UtcNow.AddDays(-6),
                Images = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/woodworking/WineBottleHolder1.jpg", IsPrimary = true }
                }
            },
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}


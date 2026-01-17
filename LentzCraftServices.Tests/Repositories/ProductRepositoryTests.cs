using Xunit;
using Microsoft.EntityFrameworkCore;
using LentzCraftServices.Infrastructure.Data;
using LentzCraftServices.Infrastructure.Repositories;
using LentzCraftServices.Domain.Entities;
using LentzCraftServices.Domain.Enums;

namespace LentzCraftServices.Tests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = ProductCategory.Woodworking,
            Status = ProductStatus.Available,
            Quantity = 1,
            Price = 100.00m,
            IsPublic = true,
            CreatedDate = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        var savedProduct = await _context.Products.FindAsync(result.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal("Test Product", savedProduct.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Category = ProductCategory.Woodworking,
            Status = ProductStatus.Available,
            Quantity = 1,
            Price = 100.00m,
            IsPublic = true,
            CreatedDate = DateTime.UtcNow
        };
        await _repository.AddAsync(product);

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetPublicProductsAsync_ShouldOnlyReturnPublicProducts()
    {
        // Arrange
        var publicProduct = new Product
        {
            Name = "Public Product",
            Category = ProductCategory.Woodworking,
            Status = ProductStatus.Available,
            Quantity = 1,
            IsPublic = true,
            CreatedDate = DateTime.UtcNow
        };
        var privateProduct = new Product
        {
            Name = "Private Product",
            Category = ProductCategory.Woodworking,
            Status = ProductStatus.Available,
            Quantity = 1,
            IsPublic = false,
            CreatedDate = DateTime.UtcNow
        };
        await _repository.AddAsync(publicProduct);
        await _repository.AddAsync(privateProduct);

        // Act
        var result = await _repository.GetPublicProductsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Public Product", result.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "Original Name",
            Category = ProductCategory.Woodworking,
            Status = ProductStatus.Available,
            Quantity = 1,
            IsPublic = true,
            CreatedDate = DateTime.UtcNow
        };
        await _repository.AddAsync(product);

        // Act
        product.Name = "Updated Name";
        await _repository.UpdateAsync(product);

        // Assert
        var updated = await _repository.GetByIdAsync(product.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        // Arrange
        var product = new Product
        {
            Name = "To Delete",
            Category = ProductCategory.Woodworking,
            Status = ProductStatus.Available,
            Quantity = 1,
            IsPublic = true,
            CreatedDate = DateTime.UtcNow
        };
        await _repository.AddAsync(product);
        var productId = product.Id;

        // Act
        await _repository.DeleteAsync(productId);

        // Assert
        var deleted = await _repository.GetByIdAsync(productId);
        Assert.Null(deleted);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

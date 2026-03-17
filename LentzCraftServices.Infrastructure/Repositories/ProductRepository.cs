using Microsoft.EntityFrameworkCore;
using LentzCraftServices.Domain.Entities;
using LentzCraftServices.Domain.Enums;
using LentzCraftServices.Infrastructure.Data;

namespace LentzCraftServices.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Product data access.
/// Provides clean abstraction over EF Core queries.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id, bool includeImages = false, bool asNoTracking = false)
    {
        var query = _context.Products.AsQueryable();
        
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(bool includeImages = false)
    {
        var query = _context.Products.AsQueryable();
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetPublicProductsAsync(bool includeImages = false)
    {
        var query = _context.Products.AsNoTracking().Where(p => p.IsPublic).AsQueryable();

        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetPublicProductsAsync(
        ProductCategory? category, ProductStatus? status, bool includeImages = false)
    {
        var query = _context.Products.AsNoTracking().Where(p => p.IsPublic);

        if (category.HasValue)
        {
            query = query.Where(p => p.Category == category.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, bool includeImages = false)
    {
        var query = _context.Products.Where(p => p.Category == category).AsQueryable();
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, bool includeImages = false)
    {
        var query = _context.Products.Where(p => p.Status == status).AsQueryable();
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, bool includeImages = false)
    {
        var pattern = $"%{searchTerm}%";
        var query = _context.Products
            .Where(p => EF.Functions.Like(p.Name, pattern) ||
                       EF.Functions.Like(p.Description, pattern))
            .AsQueryable();
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderBy(p => p.DisplayOrder).ThenByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
        var maxOrder = await _context.Products.MaxAsync(p => (int?)p.DisplayOrder) ?? 0;
        product.DisplayOrder = maxOrder + 1;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        // Find the existing tracked entity or load it
        var existingProduct = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == product.Id);
        
        if (existingProduct == null)
        {
            throw new InvalidOperationException($"Product with ID {product.Id} not found.");
        }
        
        // Update properties of the tracked entity
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Category = product.Category;
        existingProduct.Status = product.Status;
        existingProduct.Quantity = product.Quantity;
        existingProduct.Price = product.Price;
        existingProduct.IsPublic = product.IsPublic;
        existingProduct.ModifiedDate = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return existingProduct;
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<ProductImage> AddImageAsync(ProductImage image)
    {
        _context.Set<ProductImage>().Add(image);
        await _context.SaveChangesAsync();
        return image;
    }

    public async Task DeleteImageAsync(int imageId)
    {
        var image = await _context.Set<ProductImage>().FindAsync(imageId);
        if (image != null)
        {
            _context.Set<ProductImage>().Remove(image);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SetPrimaryImageAsync(int productId, int imageId)
    {
        var images = await _context.Set<ProductImage>()
            .Where(i => i.ProductId == productId)
            .ToListAsync();

        foreach (var image in images)
        {
            image.IsPrimary = image.Id == imageId;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SwapDisplayOrderAsync(int productId1, int productId2)
    {
        var product1 = await _context.Products.FindAsync(productId1);
        var product2 = await _context.Products.FindAsync(productId2);

        if (product1 == null || product2 == null)
        {
            throw new InvalidOperationException("One or both products not found for reordering.");
        }

        (product1.DisplayOrder, product2.DisplayOrder) = (product2.DisplayOrder, product1.DisplayOrder);

        await _context.SaveChangesAsync();
    }
}


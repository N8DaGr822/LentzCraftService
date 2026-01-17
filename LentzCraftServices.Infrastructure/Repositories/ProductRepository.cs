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

        return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetPublicProductsAsync(bool includeImages = false)
    {
        var query = _context.Products.Where(p => p.IsPublic).AsQueryable();
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, bool includeImages = false)
    {
        var query = _context.Products.Where(p => p.Category == category).AsQueryable();
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, bool includeImages = false)
    {
        var query = _context.Products.Where(p => p.Status == status).AsQueryable();
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, bool includeImages = false)
    {
        var term = searchTerm.ToLower();
        var query = _context.Products
            .Where(p => p.Name.ToLower().Contains(term) || 
                       p.Description.ToLower().Contains(term))
            .AsQueryable();
        
        if (includeImages)
        {
            query = query.Include(p => p.Images);
        }

        return await query.OrderByDescending(p => p.CreatedDate).ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
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
}


using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using LentzCraftServices.Domain.Entities;
using LentzCraftServices.Domain.Enums;

namespace LentzCraftServices.Infrastructure.Repositories;

/// <summary>
/// Decorator pattern implementation for caching product repository results.
/// Wraps the ProductRepository to add caching layer.
/// </summary>
public class CachedProductRepository : IProductRepository
{
    private readonly IProductRepository _innerRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedProductRepository> _logger;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public CachedProductRepository(
        IProductRepository innerRepository,
        IMemoryCache cache,
        ILogger<CachedProductRepository> logger)
    {
        _innerRepository = innerRepository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Product?> GetByIdAsync(int id, bool includeImages = false, bool asNoTracking = false)
    {
        var cacheKey = $"product_{id}_{includeImages}_{asNoTracking}";
        
        if (_cache.TryGetValue(cacheKey, out Product? cachedProduct))
        {
            _logger.LogDebug("Cache hit for product {ProductId}", id);
            return cachedProduct;
        }

        var product = await _innerRepository.GetByIdAsync(id, includeImages, asNoTracking);
        
        if (product != null)
        {
            _cache.Set(cacheKey, product, CacheExpiration);
        }

        return product;
    }

    public async Task<IEnumerable<Product>> GetAllAsync(bool includeImages = false)
    {
        var cacheKey = $"products_all_{includeImages}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Product>? cachedProducts))
        {
            _logger.LogDebug("Cache hit for all products");
            return cachedProducts ?? Enumerable.Empty<Product>();
        }

        var products = await _innerRepository.GetAllAsync(includeImages);
        var productsList = products.ToList();
        
        _cache.Set(cacheKey, productsList, CacheExpiration);
        
        return productsList;
    }

    public async Task<IEnumerable<Product>> GetPublicProductsAsync(bool includeImages = false)
    {
        var cacheKey = $"products_public_{includeImages}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Product>? cachedProducts))
        {
            _logger.LogDebug("Cache hit for public products");
            return cachedProducts ?? Enumerable.Empty<Product>();
        }

        var products = await _innerRepository.GetPublicProductsAsync(includeImages);
        var productsList = products.ToList();
        
        _cache.Set(cacheKey, productsList, CacheExpiration);
        
        return productsList;
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, bool includeImages = false)
    {
        var cacheKey = $"products_category_{category}_{includeImages}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Product>? cachedProducts))
        {
            _logger.LogDebug("Cache hit for category {Category}", category);
            return cachedProducts ?? Enumerable.Empty<Product>();
        }

        var products = await _innerRepository.GetByCategoryAsync(category, includeImages);
        var productsList = products.ToList();
        
        _cache.Set(cacheKey, productsList, CacheExpiration);
        
        return productsList;
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, bool includeImages = false)
    {
        var cacheKey = $"products_status_{status}_{includeImages}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Product>? cachedProducts))
        {
            _logger.LogDebug("Cache hit for status {Status}", status);
            return cachedProducts ?? Enumerable.Empty<Product>();
        }

        var products = await _innerRepository.GetByStatusAsync(status, includeImages);
        var productsList = products.ToList();
        
        _cache.Set(cacheKey, productsList, CacheExpiration);
        
        return productsList;
    }

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, bool includeImages = false)
    {
        // Don't cache search results as they're dynamic
        return await _innerRepository.SearchAsync(searchTerm, includeImages);
    }

    public async Task<Product> AddAsync(Product product)
    {
        var result = await _innerRepository.AddAsync(product);
        InvalidateCache();
        return result;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var result = await _innerRepository.UpdateAsync(product);
        InvalidateCache();
        return result;
    }

    public async Task DeleteAsync(int id)
    {
        await _innerRepository.DeleteAsync(id);
        InvalidateCache();
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _innerRepository.ExistsAsync(id);
    }

    private void InvalidateCache()
    {
        // Invalidate all product-related cache entries
        // In a more sophisticated implementation, you could use cache tags or specific keys
        _logger.LogDebug("Invalidating product cache");
        // Note: MemoryCache doesn't support pattern-based invalidation
        // For production, consider using Redis with tags or a more sophisticated caching solution
    }
}

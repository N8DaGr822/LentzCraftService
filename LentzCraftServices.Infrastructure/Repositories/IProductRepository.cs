using LentzCraftServices.Domain.Entities;
using LentzCraftServices.Domain.Enums;

namespace LentzCraftServices.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Product operations.
/// Follows repository pattern for clean separation of data access logic.
/// </summary>
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, bool includeImages = false, bool asNoTracking = false);
    Task<IEnumerable<Product>> GetAllAsync(bool includeImages = false);
    Task<IEnumerable<Product>> GetPublicProductsAsync(bool includeImages = false);
    Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category, bool includeImages = false);
    Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status, bool includeImages = false);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm, bool includeImages = false);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}


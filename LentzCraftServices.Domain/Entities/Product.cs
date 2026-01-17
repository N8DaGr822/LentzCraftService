using LentzCraftServices.Domain.Enums;

namespace LentzCraftServices.Domain.Entities;

/// <summary>
/// Core product entity representing a handcrafted item.
/// Supports both public portfolio display and private inventory management.
/// </summary>
public class Product
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public ProductCategory Category { get; set; }
    
    public ProductStatus Status { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Controls whether the product appears in public portfolio pages.
    /// Only products with IsPublic = true are visible to anonymous users.
    /// </summary>
    public bool IsPublic { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property for related images
    public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}


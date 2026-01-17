namespace LentzCraftServices.Domain.Entities;

/// <summary>
/// Represents an image associated with a product.
/// Supports multiple images per product with primary image designation.
/// </summary>
public class ProductImage
{
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    
    /// <summary>
    /// URL or path to the image file.
    /// In future: can be extended to support cloud storage (Azure Blob, S3, etc.)
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Indicates if this is the primary/featured image for the product.
    /// Used for gallery thumbnails and featured displays.
    /// </summary>
    public bool IsPrimary { get; set; }
    
    // Navigation property back to product
    public virtual Product Product { get; set; } = null!;
}


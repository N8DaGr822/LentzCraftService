namespace LentzCraftServices.Domain.Enums;

/// <summary>
/// Represents the availability status of a product.
/// Controls inventory management and public visibility logic.
/// </summary>
public enum ProductStatus
{
    Available = 1,      // Item is available for purchase
    Sold = 2,          // Item has been sold
    DisplayOnly = 3    // Item is for display in portfolio only
}


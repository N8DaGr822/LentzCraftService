using System.ComponentModel;

namespace LentzCraftServices.Domain.Enums;

/// <summary>
/// Represents the availability status of a product.
/// Controls inventory management and public visibility logic.
/// </summary>
public enum ProductStatus
{
    [Description("Available")]
    Available = 1,

    [Description("Sold")]
    Sold = 2,

    [Description("Display Only")]
    DisplayOnly = 3,

    [Description("Custom Order Only")]
    CustomOrderOnly = 4,

    [Description("Coming Soon")]
    ComingSoon = 5
}

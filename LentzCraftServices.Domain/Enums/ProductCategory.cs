using System.ComponentModel;

namespace LentzCraftServices.Domain.Enums;

/// <summary>
/// Represents the category of a handcrafted product.
/// Used to organize products by craft type for portfolio display.
/// </summary>
public enum ProductCategory
{
    [Description("Woodworking")]
    Woodworking = 1,

    [Description("Engraving")]
    Engraving = 2,

    [Description("Crochet")]
    Crochet = 3
}

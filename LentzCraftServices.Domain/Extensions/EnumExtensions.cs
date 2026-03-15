using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LentzCraftServices.Domain.Extensions;

/// <summary>
/// Extension methods for enums to support dynamic display names
/// using [Description] attributes.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the [Description] attribute value for an enum member,
    /// or falls back to inserting spaces before capital letters
    /// (e.g., "CustomOrderOnly" becomes "Custom Order Only").
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        if (attribute != null)
        {
            return attribute.Description;
        }

        // Fallback: insert spaces before capital letters
        return Regex.Replace(value.ToString(), "(?<!^)([A-Z])", " $1");
    }
}

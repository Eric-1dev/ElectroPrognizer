using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ElectroPrognizer.Utils.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        if (field == null)
            return string.Empty;

        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            return attribute.Description;

        return string.Empty;
    }

    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());

        if (field == null)
            return string.Empty;

        if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
            return attribute.Name;

        return string.Empty;
    }
}

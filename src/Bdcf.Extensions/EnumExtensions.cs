using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Bdcf.Extensions;

public static class EnumExtensions
{
	/// <summary>
	/// Gets the display name based on the <see cref="DisplayAttribute"/> of the enum value.
	/// If no attribute is found, the string representation of the enum value is returned.
	/// </summary>
	/// <param name="enumValue">The enum value.</param>
	/// <returns>The display name, or the string representation of the enum value.</returns>
	public static string GetDisplayName(this Enum enumValue)
	{
		var stringValue = enumValue.ToString();
		DisplayAttribute? displayAttribute = enumValue.GetType().GetMember(stringValue).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>();
		string? displayName = displayAttribute?.GetName();

		return displayName ?? stringValue;
	}
}

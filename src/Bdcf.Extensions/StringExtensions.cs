namespace Bdcf.Extensions;

public static class StringExtensions
{
	/// <summary>
	/// Returns the specified number of characters at the start of the string.
	/// If the string is shorter than the specified length, it returns the entire string.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="length">The length.</param>
	/// <returns>The shortened string.</returns>
	public static string Left(this string s, int length)
	{
		ArgumentNullException.ThrowIfNull(s);
		ArgumentOutOfRangeException.ThrowIfNegative(length);

		if (s.Length > length)
		{
			return s[..length];
		}
		else
		{
			return s;
		}
	}

	/// <summary>
	/// Trims the string and returns <see langword="null"/> when the result is empty.
	/// </summary>
	/// <param name="value">The string to trim.</param>
	/// <returns>The trimmed string, or <see langword="null"/> if the value is null, empty, or whitespace.</returns>
	public static string? TrimToNull(this string? value)
	{
		string? trimmed = value?.Trim();

		return trimmed?.Length > 0 ? trimmed : null;
	}

	/// <summary>
	/// Trims the string and returns <see cref="string.Empty"/> when the value is null.
	/// </summary>
	/// <param name="value">The string to trim.</param>
	/// <returns>The trimmed string, or <see cref="string.Empty"/> if the value is null.</returns>
	public static string TrimToEmpty(this string? value)
	{
		return value?.Trim() ?? string.Empty;
	}
}

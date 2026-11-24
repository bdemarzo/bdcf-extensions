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
	/// Trims the string, optionally limits its length, and returns <see langword="null"/> when the result is empty.
	/// </summary>
	/// <param name="value">The string to trim.</param>
	/// <param name="maxLength">The maximum length of the trimmed string. Must be at least 1 when specified.</param>
	/// <returns>The trimmed string, limited to <paramref name="maxLength"/> characters when specified, or <see langword="null"/> if the value is null, empty, or whitespace.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is less than 1.</exception>
	public static string? TrimToNull(this string? value, int? maxLength = null)
	{
		if (maxLength is < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(maxLength));
		}

		string? trimmed = value?.Trim();

		if (trimmed is null || trimmed.Length == 0)
		{
			return null;
		}

		return maxLength is int length && trimmed.Length > length ? trimmed[..length].TrimEnd() : trimmed;
	}

	/// <summary>
	/// Trims the string, optionally limits its length, and returns <see cref="string.Empty"/> when the value is null.
	/// </summary>
	/// <param name="value">The string to trim.</param>
	/// <param name="maxLength">The maximum length of the trimmed string. Must be at least 1 when specified.</param>
	/// <returns>The trimmed string, limited to <paramref name="maxLength"/> characters when specified, or <see cref="string.Empty"/> if the value is null.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> is less than 1.</exception>
	public static string TrimToEmpty(this string? value, int? maxLength = null)
	{
		if (maxLength is < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(maxLength));
		}

		string trimmed = value?.Trim() ?? string.Empty;

		return maxLength is int length && trimmed.Length > length ? trimmed[..length].TrimEnd() : trimmed;
	}
}

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
		if (s.Length > length)
		{
			return s.Substring(0, length);
		}
		else
		{
			return s;
		}
	}
}

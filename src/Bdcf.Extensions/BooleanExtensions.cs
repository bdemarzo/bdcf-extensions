namespace Bdcf.Extensions;

public static class BooleanExtensions
{
	/// <summary>
	/// If the condiiton is true, returns the specified output, otherwise returns an empty string.
	/// </summary>
	/// <param name="condition">The condition to check..</param>
	/// <param name="output">The conditional output.</param>
	/// <returns>The output if the condition is true.</returns>
	public static string Then(this bool condition, string output)
	{
		return condition ? output : string.Empty;
	}

	/// <summary>
	/// If the condiiton is false, returns the specified output, otherwise returns an empty string.
	/// </summary>
	/// <param name="condition">The condition to check..</param>
	/// <param name="output">The conditional output.</param>
	/// <returns>The output if the condition is false.</returns>
	public static string NotThen(this bool condition, string output)
	{
		return condition ? string.Empty : output;
	}
}

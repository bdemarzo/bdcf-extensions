using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace Bdcf.Extensions.Web.Routing;

/// <summary>
/// Transformer to convert API routes from pascal case to dash cased and converts it to lowercase.
/// </summary>
/// <example>api/TimeCard -> api/time-card</example>
public partial class KebabCaseOutboundParameterTransformer : IOutboundParameterTransformer
{
	/// <inheritdoc />
	public string? TransformOutbound(object? value)
	{
		return value is null ? null : WordBreaks().Replace(value.ToString() ?? string.Empty, "$1-$2").ToLower();
	}

	[GeneratedRegex(@"([^A-Z\/])([A-Z])")]
	private static partial Regex WordBreaks();
}

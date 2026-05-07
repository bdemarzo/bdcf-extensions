using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace Bdcf.Extensions.Web.Routing;

/// <summary>
/// Transformer to convert API routes from Pascal case to dash cased and converts it to lowercase.
/// </summary>
/// <example>api/TimeCard -> api/time-card</example>
public partial class KebabCaseOutboundParameterTransformer : IOutboundParameterTransformer
{
	/// <inheritdoc />
	public string? TransformOutbound(object? value)
	{
		return value is null ? null : WordBreaks().Replace(value.ToString() ?? string.Empty, "-$1$2").ToLowerInvariant();
	}

	[GeneratedRegex(@"(?<=[a-z0-9])([A-Z])|(?<=[A-Z])([A-Z])(?=[a-z])")]
	private static partial Regex WordBreaks();
}

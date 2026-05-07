namespace Bdcf.Extensions.Web.Routing.Tests;

public class KebabCaseOutboundParameterTransformerTests
{
	private readonly KebabCaseOutboundParameterTransformer _transformer = new();

	[Theory]
	[InlineData("TimeCard", "time-card")]
	[InlineData("HTMLParser", "html-parser")]
	[InlineData("MyAPIEndpoint", "my-api-endpoint")]
	[InlineData("With/SlashValue", "with/slash-value")]
	[InlineData("Already-Kebab", "already-kebab")]
	[InlineData("snake_case", "snake_case")]
	[InlineData(null, null)]
	[InlineData("", "")]
	public void TransformOutbound_ProducesExpectedKebabCase(string? input, string? expected)
	{
		var result = _transformer.TransformOutbound(input);
		Assert.Equal(expected, result);
	}
}

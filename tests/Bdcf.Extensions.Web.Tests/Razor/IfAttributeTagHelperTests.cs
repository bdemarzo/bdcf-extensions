using Bdcf.Extensions.Web.Razor;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Bdcf.Extensions.Web.Razor.Tests;

public class IfAttributeTagHelperTests
{
	[Theory]
	[InlineData(true, false, false)]
	[InlineData(false, false, true)]
	[InlineData(null, false, false)]
	[InlineData(true, true, true)]
	[InlineData(null, true, true)]
	public void Process_SuppressesOutputAccordingToConditions(bool? include, bool? exclude, bool shouldSuppress)
	{
		// Arrange
		var tagHelper = new IfAttributeTagHelper
		{
			Include = include,
			Exclude = exclude,
		};
		var context = CreateContext();
		var output = CreateOutput();

		// Act
		tagHelper.Process(context, output);

		// Assert
		Assert.Equal(shouldSuppress, output.TagName is null);
		Assert.Null(output.Attributes[IfAttributeTagHelper.INCLUDE_IF]);
		Assert.Null(output.Attributes[IfAttributeTagHelper.EXCLUDE_IF]);
	}

	private static TagHelperContext CreateContext()
	{
		return new TagHelperContext(
			new TagHelperAttributeList(),
			new Dictionary<object, object>(),
			uniqueId: "test");
	}

	private static TagHelperOutput CreateOutput()
	{
		return new TagHelperOutput(
			"div",
			new TagHelperAttributeList
			{
				new(IfAttributeTagHelper.INCLUDE_IF, true),
				new(IfAttributeTagHelper.EXCLUDE_IF, false),
			},
			(_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
	}
}

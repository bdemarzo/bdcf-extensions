using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Bdcf.Extensions.Web.Mvc.Tests;

#pragma warning disable CS0618 // These tests cover the obsolete compatibility API.

public class HtmlHelperExtensionsTests
{
	private readonly IHtmlHelper _htmlHelper = Mock.Of<IHtmlHelper>();

	[Theory]
	[InlineData(true, true)]
	[InlineData(false, false)]
	[InlineData(null, false)]
	public void If_ReturnsExpectedValue(bool? input, bool expected)
	{
		var result = _htmlHelper.If(input);
		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData(true, false)]
	[InlineData(false, true)]
	public void IfNot_ReturnsNegation(bool input, bool expected)
	{
		var result = _htmlHelper.IfNot(input);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void IfHasValue_Object_ReturnsTrue_WhenNotNull()
	{
		var result = _htmlHelper.IfHasValue(new object());
		Assert.True(result);
	}

	[Fact]
	public void IfHasValue_Object_ReturnsFalse_WhenNull()
	{
		var result = _htmlHelper.IfHasValue((object)null!);
		Assert.False(result);
	}

	[Theory]
	[InlineData("text", true)]
	[InlineData("   ", false)]
	[InlineData(null, false)]
	public void IfHasValue_String_ReturnsExpected(string? input, bool expected)
	{
		var result = _htmlHelper.IfHasValue(input);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void IfNull_ReturnsTrue_WhenNull()
	{
		var result = _htmlHelper.IfNull(null);
		Assert.True(result);
	}

	[Fact]
	public void IfNull_ReturnsFalse_WhenNotNull()
	{
		var result = _htmlHelper.IfNull("not null");
		Assert.False(result);
	}

	[Theory]
	[InlineData("Index", "Home", "Index", "Home", true)]
	[InlineData("Edit", "Product", "Details", "Product", false)]
	[InlineData("Create", "Product", "Edit", "Product", false)]
	[InlineData("Index", "Account", "Index", "Home", false)]
	public void IfActive_ReturnsExactRouteMatch(string targetAction, string targetController, string currentAction, string currentController, bool expected)
	{
		var routeData = new RouteData();
		routeData.Values["action"] = currentAction;
		routeData.Values["controller"] = currentController;

		var viewContext = new ViewContext
		{
			RouteData = routeData
		};

		var mockHtmlHelper = new Mock<IHtmlHelper>();
		mockHtmlHelper.Setup(h => h.ViewContext).Returns(viewContext);

		var result = mockHtmlHelper.Object.IfActive(targetAction, targetController);

		Assert.Equal(expected, result);
	}
}

#pragma warning restore CS0618

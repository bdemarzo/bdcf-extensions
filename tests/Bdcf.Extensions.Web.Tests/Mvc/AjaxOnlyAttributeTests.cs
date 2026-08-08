using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class AjaxOnlyAttributeTests
{
	[Fact]
	public void IsValidForRequest_ReturnsTrue_WhenRequestIsAjax()
	{
		// Arrange
		var headers = new HeaderDictionary { { "X-Requested-With", "XMLHttpRequest" } };
		var mockRequest = new Mock<HttpRequest>();
		mockRequest.Setup(r => r.Headers).Returns(headers);

		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

		var routeContext = new RouteContext(mockHttpContext.Object);
		var action = new ActionDescriptor();
		var attribute = new AjaxOnlyAttribute();

		// Act
		var result = attribute.IsValidForRequest(routeContext, action);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void IsValidForRequest_ReturnsFalse_WhenRequestIsNotAjax()
	{
		var headers = new HeaderDictionary(); // No AJAX headers
		var mockRequest = new Mock<HttpRequest>();
		mockRequest.Setup(r => r.Headers).Returns(headers);

		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

		var routeContext = new RouteContext(mockHttpContext.Object);
		var action = new ActionDescriptor();
		var attribute = new AjaxOnlyAttribute();

		var result = attribute.IsValidForRequest(routeContext, action);

		Assert.False(result);
	}

	[Fact]
	public void IsValidForRequest_ReturnsFalse_WhenRequestIsHtmx()
	{
		var headers = new HeaderDictionary { { "HX-Request", "true" } };
		var mockRequest = new Mock<HttpRequest>();
		mockRequest.Setup(r => r.Headers).Returns(headers);

		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

		var routeContext = new RouteContext(mockHttpContext.Object);
		var action = new ActionDescriptor();
		var attribute = new AjaxOnlyAttribute();

		var result = attribute.IsValidForRequest(routeContext, action);

		Assert.False(result);
	}
}

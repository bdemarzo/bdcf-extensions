using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class HtmxOnlyAttributeTests
{
	[Fact]
	public void IsValidForRequest_ReturnsTrue_WhenRequestIsHtmx()
	{
		// Arrange
		var headers = new HeaderDictionary { { "HX-Request", "true" } };
		var mockRequest = new Mock<HttpRequest>();
		mockRequest.Setup(r => r.Headers).Returns(headers);

		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

		var routeContext = new RouteContext(mockHttpContext.Object);
		var action = new ActionDescriptor();
		var attribute = new HtmxOnlyAttribute();

		// Act
		var result = attribute.IsValidForRequest(routeContext, action);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void IsValidForRequest_ReturnsFalse_WhenRequestIsNotHtmx()
	{
		var headers = new HeaderDictionary(); // No HTMX headers
		var mockRequest = new Mock<HttpRequest>();
		mockRequest.Setup(r => r.Headers).Returns(headers);

		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

		var routeContext = new RouteContext(mockHttpContext.Object);
		var action = new ActionDescriptor();
		var attribute = new HtmxOnlyAttribute();

		var result = attribute.IsValidForRequest(routeContext, action);

		Assert.False(result);
	}
}

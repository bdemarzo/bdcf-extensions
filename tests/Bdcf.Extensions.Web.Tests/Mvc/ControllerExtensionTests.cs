using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class ControllerExtensionsTests
{
	private class TestController : Controller
	{
		public TestController(HttpContext context)
		{
			ControllerContext = new ControllerContext
			{
				HttpContext = context
			};
		}
	}

	[Fact]
	public void HtmxRefresh_AddsHXRefreshHeader_AndReturnsResetContentResult()
	{
		// Arrange
		var headers = new HeaderDictionary();
		var mockResponse = new Mock<HttpResponse>();
		mockResponse.Setup(r => r.Headers).Returns(headers);

		var mockContext = new Mock<HttpContext>();
		mockContext.Setup(c => c.Response).Returns(mockResponse.Object);

		var controller = new TestController(mockContext.Object);

		// Act
		var result = controller.HtmxRefresh();

		// Assert
		Assert.IsType<ResetContentResult>(result);
		Assert.True(headers.ContainsKey("HX-Refresh"));
		Assert.Equal("true", headers["HX-Refresh"]);
	}

	[Fact]
	public void HtmxRedirect_AddsHXRedirectHeader_AndReturnsOkResult()
	{
		// Arrange
		var headers = new HeaderDictionary();
		var mockResponse = new Mock<HttpResponse>();
		mockResponse.Setup(r => r.Headers).Returns(headers);

		var mockContext = new Mock<HttpContext>();
		mockContext.Setup(c => c.Response).Returns(mockResponse.Object);

		var controller = new TestController(mockContext.Object);
		var redirectUrl = "/target";

		// Act
		var result = controller.HtmxRedirect(redirectUrl);

		// Assert
		Assert.IsType<OkResult>(result);
		Assert.True(headers.ContainsKey("HX-Redirect"));
		Assert.Equal(redirectUrl, headers["HX-Redirect"]);
	}
}

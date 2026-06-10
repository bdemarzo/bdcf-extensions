using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Bdcf.Extensions.Web.Mvc;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class HtmxRefreshResultTests
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
	public void HtmxRefresh_OnController_AddsHtmxRefreshHeader()
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
}

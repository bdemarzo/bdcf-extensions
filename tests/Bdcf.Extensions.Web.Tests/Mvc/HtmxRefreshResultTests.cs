using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class HtmxRefreshResultTests
{
	[Fact]
	public async Task ExecuteResultAsync_AddsHtmxRefreshHeader()
	{
		// Arrange
		var headers = new HeaderDictionary();
		var mockResponse = new Mock<HttpResponse>();
		mockResponse.Setup(r => r.Headers).Returns(headers);
		var mockHttpContext = new Mock<HttpContext>();
		mockHttpContext.Setup(c => c.Response).Returns(mockResponse.Object);
		var actionContext = new ActionContext
		{
			HttpContext = mockHttpContext.Object
		};
		var result = new HtmxRefreshResult();

		// Act
		await result.ExecuteResultAsync(actionContext);

		// Assert
		Assert.True(headers.ContainsKey("HX-Refresh"));
		Assert.Equal("true", headers["HX-Refresh"]);
	}
}

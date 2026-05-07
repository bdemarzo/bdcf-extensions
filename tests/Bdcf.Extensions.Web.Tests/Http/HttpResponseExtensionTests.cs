using Microsoft.AspNetCore.Http;
using Moq;

namespace Bdcf.Extensions.Web.Http.Tests;

public class HttpResponseExtensionsTests
{
	[Fact]
	public void ForceHtmxRefresh_AddsHXRefreshHeader()
	{
		// Arrange
		var headers = new HeaderDictionary();
		var mockResponse = new Mock<HttpResponse>();
		mockResponse.Setup(r => r.Headers).Returns(headers);

		// Act
		mockResponse.Object.ForceHtmxRefresh();

		// Assert
		Assert.True(headers.ContainsKey("HX-Refresh"));
		Assert.Equal("true", headers["HX-Refresh"]);
	}

	[Fact]
	public void ForceHtmxRedirect_AddsHXRedirectHeader()
	{
		// Arrange
		var headers = new HeaderDictionary();
		var mockResponse = new Mock<HttpResponse>();
		mockResponse.Setup(r => r.Headers).Returns(headers);
		var redirectUrl = "/redirect-here";

		// Act
		mockResponse.Object.ForceHtmxRedirect(redirectUrl);

		// Assert
		Assert.True(headers.ContainsKey("HX-Redirect"));
		Assert.Equal(redirectUrl, headers["HX-Redirect"]);
	}
}

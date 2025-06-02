using Microsoft.AspNetCore.Http;
using Moq;

namespace Bdcf.Extensions.Web.Http.Tests;

public class HttpRequestExtensionsTests
{
	[Fact]
	public void IsAjax_ReturnsTrue_WhenXRequestedWithIsXmlHttpRequest()
	{
		var headers = new HeaderDictionary { { "X-Requested-With", "XMLHttpRequest" } };
		var mockRequest = new Mock<HttpRequest>();
		mockRequest.Setup(r => r.Headers).Returns(headers);

		var result = mockRequest.Object.IsAjax();

		Assert.True(result);
	}

	[Fact]
	public void IsAjax_ReturnsTrue_WhenHxRequestIsTrue()
	{
		var headers = new HeaderDictionary { { "Hx-Request", "true" } };
		var mockRequest = new Mock<HttpRequest>();
		mockRequest.Setup(r => r.Headers).Returns(headers);

		var result = mockRequest.Object.IsAjax();

		Assert.True(result);
	}

	[Fact]
	public void IsAjax_ReturnsFalse_WhenNoRelevantHeadersPresent()
	{
		var headers = new HeaderDictionary();
		var mockRequest = new Mock<HttpRequest>();
		mockRequest.Setup(r => r.Headers).Returns(headers);

		var result = mockRequest.Object.IsAjax();

		Assert.False(result);
	}
}

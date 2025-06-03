using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class NoIndexAttributeTests
{
	[Fact]
	public void OnResultExecuting_AppendsXRobotsTagHeader()
	{
		var headers = new HeaderDictionary();
		var mockResponse = new Mock<HttpResponse>();
		mockResponse.Setup(r => r.Headers).Returns(headers);

		var mockContext = new Mock<HttpContext>();
		mockContext.Setup(c => c.Response).Returns(mockResponse.Object);

		var actionContext = new ActionContext
		{
			HttpContext = mockContext.Object,
			RouteData = new RouteData(),
			ActionDescriptor = new ActionDescriptor()
		};

		var resultContext = new ResultExecutingContext(
			actionContext,
			[],
			new OkResult(),
			controller: null!
		);

		var filter = new NoIndexAttribute();

		filter.OnResultExecuting(resultContext);

		Assert.True(headers.ContainsKey("X-Robots-Tag"));
		Assert.Equal("noindex", headers["X-Robots-Tag"]);
	}
}

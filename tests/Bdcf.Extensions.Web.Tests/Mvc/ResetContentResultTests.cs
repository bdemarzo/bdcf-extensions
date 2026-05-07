using Microsoft.AspNetCore.Http;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class ResetContentResultTests
{
	[Fact]
	public void Constructor_SetsStatusCodeTo205()
	{
		// Act
		var result = new ResetContentResult();

		// Assert
		Assert.Equal(StatusCodes.Status205ResetContent, result.StatusCode);
	}
}

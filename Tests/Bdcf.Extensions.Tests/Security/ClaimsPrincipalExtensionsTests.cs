using System.Security;
using System.Security.Claims;

namespace Bdcf.Extensions.Security.Tests;

public class ClaimsPrincipalExtensionsTests
{
	private ClaimsPrincipal CreatePrincipalWithClaims(params Claim[] claims)
	{
		return new ClaimsPrincipal(new ClaimsIdentity(claims));
	}

	[Theory]
	[InlineData("123", 123)]
	[InlineData("true", true)]
	[InlineData("sample@example.com", "sample@example.com")]
	public void GetClaim_ShouldReturnExpectedValue<T>(string claimValue, T expectedValue)
	{
		var claimType = "test_claim";
		var principal = CreatePrincipalWithClaims(new Claim(claimType, claimValue));

		var result = principal.GetClaim<T>(claimType);

		Assert.Equal(expectedValue, result);
	}

	[Fact]
	public void GetClaim_ShouldReturnNull_WhenClaimDoesNotExist()
	{
		var principal = CreatePrincipalWithClaims();

		var result = principal.GetClaim<int?>("non_existent_claim");

		Assert.Null(result);
	}

	[Fact]
	public void GetClaim_ShouldReturnDefault_WhenClaimIsInvalidForType()
	{
		var principal = CreatePrincipalWithClaims(new Claim("invalid_claim", "not_an_integer"));

		var result = principal.GetClaim<int>("invalid_claim");

		Assert.Equal(default, result);
	}

	[Theory]
	[InlineData("456", 456)]
	[InlineData("false", false)]
	[InlineData("admin@example.com", "admin@example.com")]
	public void GetRequiredClaim_ShouldReturnExpectedValue<T>(string claimValue, T expectedValue)
	{
		var claimType = "required_claim";
		var principal = CreatePrincipalWithClaims(new Claim(claimType, claimValue));

		var result = principal.GetRequiredClaim<T>(claimType);

		Assert.Equal(expectedValue, result);
	}

	[Fact]
	public void GetRequiredClaim_ShouldThrowException_WhenClaimIsMissing()
	{
		var principal = CreatePrincipalWithClaims();

		var exception = Assert.Throws<SecurityException>(() => principal.GetRequiredClaim<string>("missing_claim"));
		Assert.Contains("Required claim missing_claim could not be found.", exception.Message);
	}

	[Fact]
	public void GetRequiredClaim_ShouldThrowException_WhenClaimIsInvalidForType()
	{
		var principal = CreatePrincipalWithClaims(new Claim("bad_claim", "invalid_number"));

		var exception = Assert.Throws<SecurityException>(() => principal.GetRequiredClaim<int>("bad_claim"));
		Assert.Contains("Required claim bad_claim could not be converted to Int32.", exception.Message);
	}
}

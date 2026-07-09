namespace Bdcf.Extensions.Tests;

public class StringExtensionTests
{
	[Theory]
	[InlineData("123456789", 3, "123")]
	[InlineData("123456789", 10, "123456789")]
	[InlineData("123456789", 9, "123456789")]
	[InlineData("123456789", 0, "")]
	public void LeftReturnsExpectedValue(string value, int length, string expected)
	{
		Assert.Equal(expected, value.Left(length));
	}

	[Fact]
	public void LeftThrowsArgumentOutOfRangeExceptionIfLengthIsNegative()
	{
		string s = "123456789";

		Assert.Throws<ArgumentOutOfRangeException>(() => s.Left(-1));
	}

	[Fact]
	public void LeftThrowsArgumentNullExceptionIfStringIsNull()
	{
		string? s = null;

		Assert.Throws<ArgumentNullException>(() => s!.Left(1));
	}

	[Theory]
	[InlineData(null, null)]
	[InlineData("", null)]
	[InlineData("   ", null)]
	[InlineData("value", "value")]
	[InlineData("  value  ", "value")]
	[InlineData("\tvalue\r\n", "value")]
	public void TrimToNullReturnsExpectedValue(string? value, string? expected)
	{
		Assert.Equal(expected, value.TrimToNull());
	}

	[Theory]
	[InlineData(null, "")]
	[InlineData("", "")]
	[InlineData("   ", "")]
	[InlineData("value", "value")]
	[InlineData("  value  ", "value")]
	[InlineData("\tvalue\r\n", "value")]
	public void TrimToEmptyReturnsExpectedValue(string? value, string expected)
	{
		Assert.Equal(expected, value.TrimToEmpty());
	}
}

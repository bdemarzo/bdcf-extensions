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
	[InlineData("  Hello World  ", 5, "Hello")]
	[InlineData("1234 6", 5, "1234")]
	[InlineData("  12\t 34  ", 4, "12")]
	[InlineData("  Hi  ", 10, "Hi")]
	[InlineData(null, 5, null)]
	[InlineData("   ", 5, null)]
	public void TrimToNullReturnsExpectedValueWithMaxLength(string? value, int maxLength, string? expected)
	{
		Assert.Equal(expected, value.TrimToNull(maxLength));
	}

	[Theory]
	[InlineData(null, 0)]
	[InlineData(null, -1)]
	[InlineData("", 0)]
	[InlineData("", -1)]
	[InlineData("value", 0)]
	[InlineData("value", -1)]
	public void TrimToNullThrowsArgumentOutOfRangeExceptionForInvalidMaxLength(string? value, int maxLength)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => value.TrimToNull(maxLength));
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

	[Theory]
	[InlineData("  Hello World  ", 5, "Hello")]
	[InlineData("1234 6", 5, "1234")]
	[InlineData("  12\t 34  ", 4, "12")]
	[InlineData("  Hi  ", 10, "Hi")]
	[InlineData(null, 5, "")]
	[InlineData("   ", 5, "")]
	public void TrimToEmptyReturnsExpectedValueWithMaxLength(string? value, int maxLength, string expected)
	{
		Assert.Equal(expected, value.TrimToEmpty(maxLength));
	}

	[Theory]
	[InlineData(null, 0)]
	[InlineData(null, -1)]
	[InlineData("value", 0)]
	[InlineData("value", -1)]
	public void TrimToEmptyThrowsArgumentOutOfRangeExceptionForInvalidMaxLength(string? value, int maxLength)
	{
		Assert.Throws<ArgumentOutOfRangeException>(() => value.TrimToEmpty(maxLength));
	}
}

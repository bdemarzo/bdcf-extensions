namespace Bdcf.Extensions.Tests;

public class StringExtensionTests
{
	[Fact]
	public void LeftReturnsTruncatedStringIfStringLongerThanLength()
	{
		string s = "123456789";
		int length = 3;
		string expected = "123";

		Assert.Equal(expected, s.Left(length));
	}

	[Fact]
	public void LeftReturnsOriginalStringIfStringNotLongerThanLength()
	{
		string s = "123456789";
		int length = 10;
		string expected = "123456789";

		Assert.Equal(expected, s.Left(length));
	}

	[Fact]
	public void LeftReturnsOriginalStringIfLengthEqualsStringLength()
	{
		string s = "123456789";
		int length = s.Length;

		Assert.Equal(s, s.Left(length));
	}

	[Fact]
	public void LeftReturnsEmptyStringIfLengthIsZero()
	{
		string s = "123456789";

		Assert.Equal(string.Empty, s.Left(0));
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
}

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
}

namespace Bdcf.Extensions.Tests;

public class BooleanExtensionTests
{
	private const string OUTPUT = "ExpectedOutput";

	[Fact]
	public void ThenReturnsEmptyStringWhenConditionIsTrue()
	{
		Assert.Equal(OUTPUT, true.Then(OUTPUT));
	}

	[Fact]
	public void ThenReturnsOutputWhenConditionIsFalse()
	{
		Assert.Equal(string.Empty, false.Then(OUTPUT));
	}

	[Fact]
	public void NotThenReturnsEmptyStringWhenConditionIsFalse()
	{
		Assert.Equal(OUTPUT, false.NotThen(OUTPUT));
	}

	[Fact]
	public void NotThenReturnsOutputWhenConditionIsTrue()
	{
		Assert.Equal(string.Empty, true.NotThen(OUTPUT));
	}
}

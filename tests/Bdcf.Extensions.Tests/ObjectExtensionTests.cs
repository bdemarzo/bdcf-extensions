namespace Bdcf.Extensions.Tests;

public class ObjectExtensionTests
{
	[Fact]
	public void InReturnsTrueIfObjectIsInValues()
	{
		bool obj = true;
		bool[] values = { obj };

		Assert.True(obj.In(values));
	}

	[Fact]
	public void InReturnsFalseIfObjectIsNotInValues()
	{
		bool obj = true;
		bool[] values = { };

		Assert.False(obj.In(values));
	}

	[Fact]
	public void NotInReturnsTrueIfObjectIsNotInValues()
	{
		bool obj = true;
		bool[] values = { false };

		Assert.True(obj.NotIn(values));
	}

	[Fact]
	public void NotInReturnsFalseIfObjectIsInValues()
	{
		bool obj = true;
		bool[] values = { obj };

		Assert.False(obj.NotIn(values));
	}

	[Fact]
	public void BetweenReturnsTrueIfObjectIsBetweenValues()
	{
		int obj = 1;
		int start = 0;
		int end = 2;

		Assert.True(obj.Between(start, end));
	}

	[Fact]
	public void BetweenReturnsFalseIfObjectIsNotBetweenValues()
	{
		int obj = -1;
		int start = 0;
		int end = 2;

		Assert.False(obj.Between(start, end));
	}
}

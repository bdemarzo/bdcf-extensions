namespace Bdcf.Extensions.Tests;

public class DecimalExtensionTests
{
	[Fact]
	public void ToTimeSpanReturnsTimeSpanInSeconds()
	{
		decimal value = 5;
		var expected = TimeSpan.FromSeconds((double)value);

		Assert.Equal(expected, value.ToTimeSpan(TimeInterval.Seconds));
	}

	[Fact]
	public void ToTimeSpanReturnsTimeSpanInMinutes()
	{
		decimal value = 5;
		var expected = TimeSpan.FromMinutes((double)value);

		Assert.Equal(expected, value.ToTimeSpan(TimeInterval.Minutes));
	}

	[Fact]
	public void ToTimeSpanReturnsTimeSpanInHours()
	{
		decimal value = 5;
		var expected = TimeSpan.FromHours((double)value);

		Assert.Equal(expected, value.ToTimeSpan(TimeInterval.Hours));
	}

	[Fact]
	public void ToTimeSpanReturnsNullIfValueIsNull()
	{
		decimal? value = null;
		TimeSpan? expected = null;

		Assert.Equal(expected, value.ToTimeSpan(TimeInterval.Seconds));
		Assert.Equal(expected, value.ToTimeSpan(TimeInterval.Minutes));
		Assert.Equal(expected, value.ToTimeSpan(TimeInterval.Hours));
	}
}

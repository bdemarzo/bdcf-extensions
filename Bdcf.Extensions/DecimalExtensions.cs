namespace Bdcf.Extensions;

public static class DecimalExtensions
{
	/// <summary>
	/// Converts a decimal to a <see cref="TimeSpan"/>>, with the decimal value being used as the specified time interval.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="interval">The interval.</param>
	/// <returns>The time span.</returns>
	public static TimeSpan ToTimeSpan(this decimal value, TimeInterval interval)
	{
		return interval switch
		{
			TimeInterval.Seconds => TimeSpan.FromSeconds((double)value),
			TimeInterval.Minutes => TimeSpan.FromMinutes((double)value),
			TimeInterval.Hours => TimeSpan.FromHours((double)value),
			_ => throw new NotImplementedException($"Unsupported TimeInterval {interval}")
		};
	}

	/// <summary>
	/// Converts a decimal to a <see cref="TimeSpan"/>>, with the decimal value being used as the specified time interval.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="interval">The interval.</param>
	/// <returns>The time span, or null if the source value was null.</returns>
	public static TimeSpan? ToTimeSpan(this decimal? value, TimeInterval interval)
	{
		if (value is null)
			return null;

		return value.Value.ToTimeSpan(interval);
	}
}

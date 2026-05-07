namespace Bdcf.Extensions.DataAnnotations;

/// <summary>
/// Defines the smallest <see cref="TimeSpan"/> unit preserved by <see cref="PrecisionAttribute"/>.
/// </summary>
public enum TimeSpanPrecision
{
	/// <summary>
	/// Preserve the original value.
	/// </summary>
	None,

	/// <summary>
	/// Preserve values down to whole seconds.
	/// </summary>
	Seconds,

	/// <summary>
	/// Preserve values down to whole minutes.
	/// </summary>
	Minutes,

	/// <summary>
	/// Preserve values down to whole hours.
	/// </summary>
	Hours
}

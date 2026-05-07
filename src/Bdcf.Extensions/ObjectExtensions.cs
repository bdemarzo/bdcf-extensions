namespace Bdcf.Extensions;

public static class ObjectExtensions
{
	/// <summary>
	/// Returns true if the value is represented in the provided set.
	/// </summary>
	/// <typeparam name="T">Type of the value</typeparam>
	/// <param name="obj">The object to check if the set contains</param>
	/// <param name="values">The set that might contain the object</param>
	/// <returns>True if the object exists in the set</returns>
	public static bool In<T>(this T obj, params T[] values)
	{
		return values.Contains(obj);
	}

	/// <summary>
	/// Returns true if the value is not represented in the provided set.
	/// </summary>
	/// <typeparam name="T">Type of the value</typeparam>
	/// <param name="obj">The object to check if the set contains</param>
	/// <param name="values">The set that might contain the object</param>
	/// <returns>True if the object does not exist in the set</returns>
	public static bool NotIn<T>(this T obj, params T[] values)
	{
		return !(obj.In(values));
	}

	/// <summary>
	/// Given two <see cref="IComparable"/> objects, will return true if the object is between the <paramref name="start"/> and <paramref name="end"/> objects.
	/// The between is inclusive, so if the object is equal to the start or end, it will return true.
	/// </summary>
	/// <typeparam name="T">Type of the value</typeparam>
	/// <param name="obj">The object to check</param>
	/// <param name="start">The start of the range</param>
	/// <param name="end">The end of the range</param>
	/// <returns>True if the object is in the range between start and end, false otherwise.</returns>
	public static bool Between<T>(this T obj, T start, T end) where T : IComparable
	{
		return (obj.CompareTo(start) >= 0 && obj.CompareTo(end) <= 0);
	}
}

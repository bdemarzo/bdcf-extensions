using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;

namespace Bdcf.Extensions.Web.Mvc;

public static class TempDataExtensions
{
	/// <summary>
	/// Serializes an object and places it in temp data.
	/// </summary>
	/// <typeparam name="T">The object type</typeparam>
	/// <param name="tempData">The data to store.</param>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
	{
		tempData[key] = JsonSerializer.Serialize(value);
	}

	/// <summary>
	/// Deserializes and reconstitutes an object from temp datea.
	/// </summary>
	/// <typeparam name="T">The object type</typeparam>
	/// <param name="tempData">The temporary data.</param>
	/// <param name="key">The key.</param>
	/// <returns>The object</returns>
	public static T? Get<T>(this ITempDataDictionary tempData, string key) where T : class
	{
		tempData.TryGetValue(key, out object? o);
		return o is null ? null : JsonSerializer.Deserialize<T>((string)o);
	}
}

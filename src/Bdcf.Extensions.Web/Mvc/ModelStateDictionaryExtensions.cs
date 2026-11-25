using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bdcf.Extensions.Web.Mvc;

public static class ModelStateDictionaryExtensions
{
	/// <summary>
	/// Adds multiple errors to the model state entry for the specified key.
	/// </summary>
	/// <param name="modelState">The model state dictionary.</param>
	/// <param name="key">The key for the model state entry.</param>
	/// <param name="errors">The errors to add.</param>
	public static void AddModelErrors(this ModelStateDictionary modelState, string key, string[] errors)
	{
		foreach (var error in errors)
		{
			modelState.AddModelError(key, error);
		}
	}
}

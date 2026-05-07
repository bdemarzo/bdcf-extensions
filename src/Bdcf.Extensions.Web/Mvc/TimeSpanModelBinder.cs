using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bdcf.Extensions.Web.Mvc;

public class TimeSpanModelBinder : IModelBinder
{
	private static readonly string[] TimeSpanFormats =
	[
		@"h\:mm",
		@"hh\:mm",
		@"h\:mm\:ss",
		@"hh\:mm\:ss"
	];

	/// <summary>
	/// Attempts to bind a model of type <see cref="TimeSpan"/> from the provided input value.
	/// </summary>
	/// <remarks>
	/// Accepted values are invariant clock-style durations using <c>h:mm</c>, <c>hh:mm</c>, <c>h:mm:ss</c>, or
	/// <c>hh:mm:ss</c>, and decimal hour values parsed with the value provider culture.
	/// </remarks>
	/// <param name="bindingContext">The <see cref="ModelBindingContext"/> containing the model name, value provider, and other context information
	/// required for model binding. This parameter cannot be <see langword="null"/>.</param>
	/// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task result contains the binding outcome,
	/// which may be a successfully bound <see cref="TimeSpan"/> instance or an error added to the model state.</returns>
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

		if (valueProviderResult != ValueProviderResult.None)
		{
			var value = valueProviderResult.FirstValue;

			if (value is null)
			{
				bindingContext.Result = ModelBindingResult.Success(null);
			}
			else if (TimeSpan.TryParseExact(value, TimeSpanFormats, CultureInfo.InvariantCulture, out TimeSpan timeSpan))
			{
				bindingContext.Result = ModelBindingResult.Success(timeSpan);
			}
			else if (double.TryParse(value, NumberStyles.Float, valueProviderResult.Culture, out double hours))
			{
				bindingContext.Result = ModelBindingResult.Success(TimeSpan.FromHours(hours));
			}
			else
			{
				bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Invalid time format");
			}
		}

		return Task.CompletedTask;
	}
}

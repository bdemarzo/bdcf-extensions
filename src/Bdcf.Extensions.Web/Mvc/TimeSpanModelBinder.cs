using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bdcf.Extensions.Web.Mvc;

public class TimeSpanModelBinder : IModelBinder
{
	/// <summary>
	/// Attempts to bind a model of type <see cref="TimeSpan"/> from the provided input value.
	/// </summary>
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
			else if (TimeSpan.TryParseExact(value, @"h\:mm", null, out TimeSpan timeSpan))
			{
				bindingContext.Result = ModelBindingResult.Success(timeSpan);
			}
			else if (double.TryParse(value, out double hours))
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

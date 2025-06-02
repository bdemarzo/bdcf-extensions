using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Gigkeeping.Web;

public class TimeSpanModelBinder : IModelBinder
{
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

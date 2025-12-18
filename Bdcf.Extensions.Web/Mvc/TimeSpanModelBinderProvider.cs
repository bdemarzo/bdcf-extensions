using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bdcf.Extensions.Web.Mvc;

public class TimeSpanModelBinderProvider : IModelBinderProvider
{
	/// <summary>
	/// Retrieves a model binder for the specified context if the model type is <see cref="TimeSpan?"/>.
	/// </summary>
	/// <param name="context">The <see cref="ModelBinderProviderContext"/> that provides metadata about the model to be bound.</param>
	/// <returns>An instance of <see cref="IModelBinder"/> for binding <see cref="TimeSpan?"/> values,  or <see langword="null"/> if
	/// the model type is not <see cref="TimeSpan?"/>.</returns>
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		if (context.Metadata.ModelType == typeof(TimeSpan) || context.Metadata.ModelType == typeof(TimeSpan?))
		{
			return new TimeSpanModelBinder();
		}
		return null;
	}
}

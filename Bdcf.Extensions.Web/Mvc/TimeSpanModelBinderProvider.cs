using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bdcf.Extensions.Web;

public class TimeSpanModelBinderProvider : IModelBinderProvider
{
	public IModelBinder? GetBinder(ModelBinderProviderContext context)
	{
		if (context.Metadata.ModelType == typeof(TimeSpan?))
		{
			return new TimeSpanModelBinder();
		}
		return null;
	}
}

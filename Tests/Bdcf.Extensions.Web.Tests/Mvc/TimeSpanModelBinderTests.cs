using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System.Globalization;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class TimeSpanModelBinderTests
{
	[Fact]
	public async Task BindModelAsync_NullValue_SetsResultToSuccessWithNull()
	{
		// Arrange
		var valueProvider = new Mock<IValueProvider>();
		valueProvider.Setup(vp => vp.GetValue("test")).Returns(new ValueProviderResult());
		var bindingContext = GetBindingContext(valueProvider.Object, "test");

		var binder = new TimeSpanModelBinder();

		// Act
		await binder.BindModelAsync(bindingContext);

		// Assert
		Assert.False(bindingContext.Result.IsModelSet);
		Assert.Null(bindingContext.Result.Model);
	}

	[Fact]
	public async Task BindModelAsync_ValidTimeSpanString_SetsResultToSuccessWithTimeSpan()
	{
		// Arrange
		var valueProvider = new Mock<IValueProvider>();
		valueProvider.Setup(vp => vp.GetValue("test")).Returns(new ValueProviderResult("2:30"));
		var bindingContext = GetBindingContext(valueProvider.Object, "test");

		var binder = new TimeSpanModelBinder();

		// Act
		await binder.BindModelAsync(bindingContext);

		// Assert
		Assert.True(bindingContext.Result.IsModelSet);
		Assert.Equal(TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30), bindingContext.Result.Model);
	}

	[Fact]
	public async Task BindModelAsync_ValidTimeSpanWithSeconds_SetsResultToSuccessWithTimeSpan()
	{
		// Arrange
		var valueProvider = new Mock<IValueProvider>();
		valueProvider.Setup(vp => vp.GetValue("test")).Returns(new ValueProviderResult("2:30:15"));
		var bindingContext = GetBindingContext(valueProvider.Object, "test");

		var binder = new TimeSpanModelBinder();

		// Act
		await binder.BindModelAsync(bindingContext);

		// Assert
		Assert.True(bindingContext.Result.IsModelSet);
		Assert.Equal(TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30) + TimeSpan.FromSeconds(15), bindingContext.Result.Model);
	}

	[Fact]
	public async Task BindModelAsync_ValidDoubleString_SetsResultToSuccessWithTimeSpan()
	{
		// Arrange
		var valueProvider = new Mock<IValueProvider>();
		valueProvider.Setup(vp => vp.GetValue("test")).Returns(new ValueProviderResult("1.5"));
		var bindingContext = GetBindingContext(valueProvider.Object, "test");

		var binder = new TimeSpanModelBinder();

		// Act
		await binder.BindModelAsync(bindingContext);

		// Assert
		Assert.True(bindingContext.Result.IsModelSet);
		Assert.Equal(TimeSpan.FromHours(1.5), bindingContext.Result.Model);
	}

	[Fact]
	public async Task BindModelAsync_ValidDoubleString_UsesValueProviderCulture()
	{
		// Arrange
		var valueProvider = new Mock<IValueProvider>();
		valueProvider.Setup(vp => vp.GetValue("test")).Returns(new ValueProviderResult("1,5", CultureInfo.GetCultureInfo("fr-FR")));
		var bindingContext = GetBindingContext(valueProvider.Object, "test");

		var binder = new TimeSpanModelBinder();

		// Act
		await binder.BindModelAsync(bindingContext);

		// Assert
		Assert.True(bindingContext.Result.IsModelSet);
		Assert.Equal(TimeSpan.FromHours(1.5), bindingContext.Result.Model);
	}

	[Fact]
	public async Task BindModelAsync_InvalidString_AddsModelError()
	{
		// Arrange
		var valueProvider = new Mock<IValueProvider>();
		valueProvider.Setup(vp => vp.GetValue("test")).Returns(new ValueProviderResult("invalid"));
		var bindingContext = GetBindingContext(valueProvider.Object, "test");

		var binder = new TimeSpanModelBinder();

		// Act
		await binder.BindModelAsync(bindingContext);

		// Assert
		Assert.False(bindingContext.Result.IsModelSet);
		Assert.True(bindingContext.ModelState.ContainsKey("test"));
		var error = bindingContext.ModelState["test"]?.Errors[0].ErrorMessage;
		Assert.Equal("Invalid time format", error);
	}

	private static DefaultModelBindingContext GetBindingContext(IValueProvider valueProvider, string modelName)
	{
		return new DefaultModelBindingContext
		{
			ModelName = modelName,
			ValueProvider = valueProvider,
			ModelState = new ModelStateDictionary()
		};
	}
}

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class ModelStateDictionaryExtensionsTests
{
	[Fact]
	public void AddModelErrors_AddsEachErrorForTheSpecifiedKey()
	{
		var modelState = new ModelStateDictionary();
		var errors = new[] { "First error", "Second error" };

		modelState.AddModelErrors("key", errors);

		Assert.Collection(
			modelState["key"]!.Errors,
			error => Assert.Equal("First error", error.ErrorMessage),
			error => Assert.Equal("Second error", error.ErrorMessage));
	}

	[Fact]
	public void AddModelErrors_PreservesExistingErrors()
	{
		var modelState = new ModelStateDictionary();
		modelState.AddModelError("key", "Existing error");

		modelState.AddModelErrors("key", ["New error"]);

		Assert.Collection(
			modelState["key"]!.Errors,
			error => Assert.Equal("Existing error", error.ErrorMessage),
			error => Assert.Equal("New error", error.ErrorMessage));
	}

	[Fact]
	public void AddModelErrors_WithEmptyArray_DoesNotAddErrors()
	{
		var modelState = new ModelStateDictionary();

		modelState.AddModelErrors("key", []);

		Assert.False(modelState.ContainsKey("key"));
	}
}

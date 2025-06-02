using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Text.Json;

namespace Bdcf.Extensions.Web.Mvc.Tests;

public class TempDataExtensionsTests
{
	private class SampleData
	{
		public string Name { get; set; } = "";
		public int Age { get; set; }
	}

	[Fact]
	public void Put_SerializesObjectToJson()
	{
		// Arrange
		var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
		var data = new SampleData { Name = "Brian", Age = 42 };

		// Act
		tempData.Put("test", data);

		// Assert
		Assert.True(tempData.ContainsKey("test"));
		var expectedJson = JsonSerializer.Serialize(data);
		Assert.Equal(expectedJson, tempData["test"]);
	}

	[Fact]
	public void Get_DeserializesJsonToObject()
	{
		// Arrange
		var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
		var original = new SampleData { Name = "Alice", Age = 30 };
		tempData["user"] = JsonSerializer.Serialize(original);

		// Act
		var result = tempData.Get<SampleData>("user");

		// Assert
		Assert.NotNull(result);
		Assert.Equal(original.Name, result!.Name);
		Assert.Equal(original.Age, result.Age);
	}

	[Fact]
	public void Get_ReturnsNull_WhenKeyNotFound()
	{
		var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

		var result = tempData.Get<SampleData>("missing");

		Assert.Null(result);
	}
}

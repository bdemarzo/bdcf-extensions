using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.Tests;

public class EnumExtensionTests
{
	private enum TestEnum
	{
		NoDisplayName,
		[Display(Name = "Has a display name")]
		HasDisplayName,
		NoDescription,
		[System.ComponentModel.Description("Has a description")]
		HasDescription
	}

	[Flags]
	private enum TestFlags
	{
		None = 0,
		First = 1,
		Second = 2
	}

	[Fact]
	public void GetDisplayNameReturnsDisplayAttributeIfExists()
	{
		var value = TestEnum.HasDisplayName;
		var expected = "Has a display name";

		Assert.Equal(expected, value.GetDisplayName());
	}

	[Fact]
	public void GetDisplayNameReturnsToStringIfNoDisplayAttributeExists()
	{
		var value = TestEnum.NoDisplayName;
		var expected = "NoDisplayName";

		Assert.Equal(expected, value.GetDisplayName());
	}

	[Fact]
	public void GetDisplayNameReturnsToStringForUndefinedEnumValue()
	{
		var value = (TestEnum)42;

		Assert.Equal("42", value.GetDisplayName());
	}

	[Fact]
	public void GetDisplayNameReturnsToStringForCombinedFlagValue()
	{
		var value = TestFlags.First | TestFlags.Second;

		Assert.Equal("First, Second", value.GetDisplayName());
	}

	[Fact]
	public void GetDescriptionReturnsDescriptionAttributeIfExists()
	{
		Assert.Equal("Has a description", TestEnum.HasDescription.GetDescription());
	}

	[Fact]
	public void GetDescriptionReturnsToStringIfNoDescriptionAttributeExists()
	{
		Assert.Equal("NoDescription", TestEnum.NoDescription.GetDescription());
	}

	[Fact]
	public void GetDescriptionReturnsToStringForUndefinedEnumValue()
	{
		Assert.Equal("42", ((TestEnum)42).GetDescription());
	}
}

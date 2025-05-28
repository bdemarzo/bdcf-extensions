using System.ComponentModel.DataAnnotations;

namespace Bdcf.Extensions.Tests;

public class EnumExtensionTests
{
	private enum TestEnum
	{
		NoDisplayName,
		[Display(Name = "Has a display name")]
		HasDisplayName
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
}

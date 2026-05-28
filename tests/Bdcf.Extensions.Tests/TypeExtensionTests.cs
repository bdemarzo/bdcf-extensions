namespace Bdcf.Extensions.Tests;

public class TypeExtensionTests
{
	private class Parent { }
	private class Child : Parent { }
	private class Grandchild : Child { }

	[Fact]
	public void IsDirectDescendantOfReturnsTrueIfObjectIsDirectDescendant()
	{
		var dereivedType = new Child();
		var baseType = new Parent();

		Assert.True(dereivedType.GetType().IsDirectDescendantOf(baseType.GetType()));
	}

	[Fact]
	public void IsDirectDescendantOfReturnsFalseIfObjectIsNotDirectDescendant()
	{
		var derivedType = new Grandchild();
		var baseType = new Parent();

		Assert.False(derivedType.GetType().IsDirectDescendantOf(baseType.GetType()));
	}

	[Fact]
	public void IsDirectDescendantOfReturnsFalseIfObjectIsNotDescendant()
	{
		var derivedType = new object();
		var baseType = new Parent();

		Assert.False(derivedType.GetType().IsDirectDescendantOf(baseType.GetType()));
	}
}

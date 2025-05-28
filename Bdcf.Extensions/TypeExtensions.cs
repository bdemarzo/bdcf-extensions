namespace Bdcf.Extensions;

public static class TypeExtensions
{
	/// <summary>
	/// Determines whether <paramref name="derivedType"/> is a direct descendant of <paramref name="baseType"/>.
	/// </summary>
	/// <param name="derivedType">Derived type to check.</param>
	/// <param name="baseType">Base type to check against.</param>
	/// <returns>
	///   <c>true</c> if <paramref name="derivedType"/> is a direct descendant of <paramref name="baseType"/>; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsDirectDescendantOf(this Type derivedType, Type baseType)
    {
        return derivedType.BaseType == baseType;
    }
}

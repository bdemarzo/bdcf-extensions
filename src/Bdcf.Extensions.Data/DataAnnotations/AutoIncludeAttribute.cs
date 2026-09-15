namespace Bdcf.Extensions.Data.DataAnnotations;

/// <summary>
/// Apply EF AutoInclude() to the attributed navigation property
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AutoIncludeAttribute : Attribute
{
}

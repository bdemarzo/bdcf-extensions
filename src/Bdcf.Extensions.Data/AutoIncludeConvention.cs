using Bdcf.Extensions.Data.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace Bdcf.Extensions.Data;

/// <summary>
/// EF convention that enables AutoInclude for all navigation properties that have the <see cref="AutoIncludeAttribute"/>
/// </summary>
internal class AutoIncludeConvention : NavigationAttributeConventionBase<AutoIncludeAttribute>, INavigationAddedConvention, ISkipNavigationAddedConvention
{
	/// <inheritdoc/>
	public AutoIncludeConvention(ProviderConventionSetBuilderDependencies dependencies) : base(dependencies) { }

	/// <inheritdoc/>
	public override void ProcessNavigationAdded(IConventionNavigationBuilder navigationBuilder, AutoIncludeAttribute attribute, IConventionContext<IConventionNavigationBuilder> context)
	{
		navigationBuilder.AutoInclude(true, true);
	}

	/// <inheritdoc/>
	public override void ProcessSkipNavigationAdded(IConventionSkipNavigationBuilder skipNavigationBuilder, AutoIncludeAttribute attribute, IConventionContext<IConventionSkipNavigationBuilder> context)
	{
		skipNavigationBuilder.AutoInclude(true, true);
	}
}

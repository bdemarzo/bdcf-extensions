using System.Reflection;

namespace Bdcf.Extensions.DacPac;

/// <summary>
/// Represents configuration options for deploying a Data-tier Application Component (DAC) package.
/// </summary>
public record DacPacOptions
{
	/// <summary>
	/// Gets the connection string used to establish a connection to the database.
	/// If <see cref="ConnectionStringName"/> is specified and found, it takes precedence over this value.
	/// If the connection string name is not found or empty, this value will be used instead.
	/// </summary>
	public string ConnectionString { get; init; } = string.Empty;

	/// <summary>
	/// Gets the connection string name to use to establish a connection to the database.
	/// If this is specified, it takes precedence over <see cref="ConnectionString"/>.
	/// If the connection string is not found or empty, <see cref="ConnectionString"/> will be used instead.
	/// </summary>
	public string ConnectionStringName { get; init; } = string.Empty;

	/// <summary>
	/// Specifies the name of the assembly where the dacpac file is stored (as an embedded resource).
	/// This is useful for configuration-bound options.
	/// </summary>
	public string? AssemblyName { get; init; }

	/// <summary>
	/// Specifies the assembly where the dacpac file is stored as an embedded resource.
	/// This is preferred when configuring options in code.
	/// </summary>
	public Assembly? DacPacAssembly { get; init; }

	/// <summary>
	/// Specifies a type in the assembly where the dacpac file is stored as an embedded resource.
	/// This is a convenient alternative to setting <see cref="DacPacAssembly"/> directly.
	/// </summary>
	public Type? DacPacResourceMarkerType { get; init; }

	/// <summary>
	/// Specifies the name of the dacpac file as it is stored in the assembly.
	/// </summary>
	public required string DacPacName { get; init; }

	/// <summary>
	/// Specifies a value indicating whether the operation should block execution when potential data loss is detected.
	/// Defaults to true.
	/// </summary>
	public bool BlockOnPossibleDataLoss { get; init; } = true;

	/// <summary>
	/// Specifies a value indicating whether smart default values should be automatically generated.
	/// </summary>
	public bool GenerateSmartDefaults { get; init; } = false;

	/// <summary>
	/// Specifies the timeout in seconds for long-running commands during the deployment process. 
	/// Defaults to 0, indicating no timeout.
	/// </summary>
	public int LongRunningCommandTimeout { get; init; } = 0;

	/// <summary>
	/// Specifies a value indicating whether objects that are not present in the source should be dropped during deployment.
	/// </summary>
	public bool DropObjectsNotInSource { get; init; } = false;

	/// <summary>
	/// Specifies a value indicating whether the deployment should be verified after it is applied.
	/// Defaults to true.
	/// </summary>
	public bool VerifyDeployment { get; init; } = true;
}

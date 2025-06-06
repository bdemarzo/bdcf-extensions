namespace Bdcf.Extensions.DacPac;

/// <summary>
/// Represents configuration options for deploying a Data-tier Application Component (DAC) package.
/// </summary>
public record DacPacOptions
{
	/// <summary>
	/// Gets the connection string used to establish a connection to the database.
	/// </summary>
	public required string ConnectionString { get; init; }

	/// <summary>
	/// Specifies the name of the assembly where the dacpac file is stored (as an embedded resource).
	/// If left null, the current executing assembly will be used.
	/// </summary>
	public string? AssemblyName { get; init; }

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

namespace Bdcf.Extensions.DacPac;

/// <summary>
/// Interface for a service that applies a Data-tier Application Component (DAC) package (DacPac) to a database.
/// </summary>
public interface IDacPacService
{
	/// <summary>
	/// Deploys a Data-tier Application Component Package (DACPAC) to the target database specified in the connection
	/// string.
	/// </summary>
	void ApplyDacPac();
}

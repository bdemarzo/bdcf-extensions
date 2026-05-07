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

	/// <summary>
	/// Deploys a Data-tier Application Component Package (DACPAC) asynchronously.
	/// </summary>
	/// <remarks>
	/// The underlying DacFx deployment API is synchronous, so cancellation is honored before scheduling deployment work.
	/// </remarks>
	/// <param name="cancellationToken">A token used to cancel deployment before it starts.</param>
	/// <returns>A task that completes when deployment finishes.</returns>
	Task ApplyDacPacAsync(CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		return Task.Run(ApplyDacPac, cancellationToken);
	}
}

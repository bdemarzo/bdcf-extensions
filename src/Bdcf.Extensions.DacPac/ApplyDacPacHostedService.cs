using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bdcf.Extensions.DacPac;

internal sealed class ApplyDacPacHostedService : IHostedService
{
	private readonly IDacPacService _dacPacService;
	private readonly ILogger<ApplyDacPacHostedService> _logger;
	private readonly bool _failOnError;

	public ApplyDacPacHostedService(IDacPacService dacPacService, ILogger<ApplyDacPacHostedService> logger, IOptions<ApplyDacPacHostedServiceOptions> options)
	{
		_dacPacService = dacPacService;
		_logger = logger;
		_failOnError = options?.Value?.FailOnError ?? false;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Applying DACPAC at startup...");
		try
		{
			await _dacPacService.ApplyDacPacAsync(cancellationToken).ConfigureAwait(false);
			_logger.LogInformation("DACPAC applied successfully at startup.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while applying DACPAC at startup.");
			if (_failOnError)
			{
				// bubbling the exception will prevent the host from starting
				throw;
			}
		}
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Dac;
using System.Data.Common;
using System.Reflection;

namespace Bdcf.Extensions.DacPac;

public class DacPacService : IDacPacService
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<DacPacService> _logger;
	private const string DACPAC_NAME = "Gigkeeping.Data.DacPac.Gigkeeping.Database.dacpac";

	public DacPacService(IConfiguration configuration, ILogger<DacPacService> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public void ApplyDacPac()
	{
		string connectionString = _configuration.GetConnectionString("DataContext") ?? throw new InvalidOperationException("Unable to find ConnectionString 'DataContext'");
		string databaseName = GetDatabaseNameFromConnectionString(connectionString);

		// Load the embedded DACPAC
		var assembly = Assembly.GetExecutingAssembly();
		using Stream dacPacStream = assembly.GetManifestResourceStream(DACPAC_NAME) ?? throw new FileNotFoundException($"Unable to load embedded DacPac {DACPAC_NAME}");

		var dacServices = new DacServices(connectionString);
		dacServices.Message += (sender, e) =>
		{
			var message = e.Message;
			switch (message.MessageType)
			{
				case DacMessageType.Error:
					_logger.LogError(message.Message);
					break;
				case DacMessageType.Warning:
					_logger.LogWarning(message.Message);
					break;
				default:
					_logger.LogInformation(message.Message);
					break;
			}
		};
		DacDeployOptions deployOptions = new()
		{
			BlockOnPossibleDataLoss = false,
			GenerateSmartDefaults = true,
			LongRunningCommandTimeout = 60,
			DropObjectsNotInSource = true,
			VerifyDeployment = true
		};

		using DacPackage dacPackage = DacPackage.Load(dacPacStream);
		_logger.LogInformation("Starting DACPAC deployment...");
		dacServices.Deploy(dacPackage, databaseName, true, deployOptions);
		_logger.LogInformation("DACPAC deployment completed.");
	}

	private static string GetDatabaseNameFromConnectionString(string connectionString)
	{
		var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
		if (builder.TryGetValue("Initial Catalog", out var databaseName))
		{
			return databaseName.ToString()!;
		}
		else if (builder.TryGetValue("Database", out databaseName))
		{
			return databaseName.ToString()!;
		}

		throw new InvalidOperationException("The connection string does not contain a database name.");
	}
}

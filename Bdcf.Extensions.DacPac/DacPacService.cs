using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Dac;
using System.Data.Common;
using System.Reflection;
using System.Configuration;

namespace Bdcf.Extensions.DacPac;

/// <summary>
/// Provides functionality for deploying a DACPAC (Data-tier Application Component Package) to a database.
/// </summary>
public class DacPacService : IDacPacService
{
	private readonly DacPacOptions _options;
	private readonly ILogger<DacPacService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="DacPacService"/> class.
	/// </summary>
	/// <param name="options">The configuration options for the DacPac service.</param>
	/// <param name="logger">The logger instance used to log diagnostic and operational information.</param>
	public DacPacService(IOptions<DacPacOptions> options, ILogger<DacPacService> logger)
	{
		_options = options.Value;
		_logger = logger;
	}

	/// <summary>
	/// Deploys a Data-tier Application Component Package (DACPAC) to the target database specified in the connection
	/// string.
	/// </summary>
	public void ApplyDacPac()
	{
		var connectionString = _options.ConnectionString;
		if (!string.IsNullOrWhiteSpace(_options.ConnectionStringName))
		{
			var namedConnectionString = ConfigurationManager.ConnectionStrings[_options.ConnectionStringName];
			if (!string.IsNullOrWhiteSpace(namedConnectionString?.ConnectionString))
			{
				connectionString = namedConnectionString.ConnectionString;
			}
		}
		var databaseName = GetDatabaseNameFromConnectionString(connectionString);
		if (string.IsNullOrWhiteSpace(databaseName))
			throw new InvalidOperationException("Database name could not be extracted from the connection string.");

		var dacPacStream = GetDacPacStream(_options.AssemblyName, _options.DacPacName);

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
			BlockOnPossibleDataLoss = _options.BlockOnPossibleDataLoss,
			GenerateSmartDefaults = _options.GenerateSmartDefaults,
			LongRunningCommandTimeout = _options.LongRunningCommandTimeout,
			DropObjectsNotInSource = _options.DropObjectsNotInSource,
			VerifyDeployment = _options.VerifyDeployment
		};

		using DacPackage dacPackage = DacPackage.Load(dacPacStream);
		_logger.LogInformation("Starting DACPAC deployment...");
		dacServices.Deploy(dacPackage, databaseName, true, deployOptions);
		_logger.LogInformation("DACPAC deployment completed.");
	}

	/// <summary>
	/// Extracts the database name from the provided connection string.
	/// If the database name can not be extracted, null is returned.
	/// </summary>
	/// <param name="connectionString">A connection string.</param>
	/// <returns>The database name parsed from the connection string.</returns>
	protected static string? GetDatabaseNameFromConnectionString(string connectionString)
	{
		var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
		if (builder.TryGetValue("Initial Catalog", out var databaseName))
		{
			return databaseName.ToString();
		}
		else if (builder.TryGetValue("Database", out databaseName))
		{
			return databaseName.ToString();
		}

		return null;
	}

	/// <summary>
	/// Retrieves a stream for an embedded DacPac resource from the specified assembly.
	/// </summary>
	/// <param name="assemblyName">The name of the assembly containing the embedded DacPac resource. If <see langword="null"/>,  the currently
	/// executing assembly is used.</param>
	/// <param name="dacPacName">The name of the embedded DacPac resource to retrieve. This must match the resource name exactly.</param>
	/// <returns>A <see cref="Stream"/> representing the embedded DacPac resource.</returns>
	/// <exception cref="FileNotFoundException">Thrown if the specified DacPac resource cannot be found in the assembly.</exception>
	protected static Stream GetDacPacStream(string? assemblyName, string dacPacName)
	{
		var assembly = assemblyName is null ? Assembly.GetExecutingAssembly() : Assembly.Load(assemblyName);

		return assembly.GetManifestResourceStream(dacPacName) ?? throw new FileNotFoundException($"Unable to load embedded DacPac {dacPacName}");
	}

	/// <summary>
	/// Creates and configures a <see cref="DacDeployOptions"/> instance based on the specified <see
	/// cref="DacPacOptions"/>.
	/// </summary>
	/// <param name="options">The <see cref="DacPacOptions"/> containing deployment settings to apply.</param>
	/// <returns>A <see cref="DacDeployOptions"/> instance configured with the values from <paramref name="options"/>.</returns>
	protected static DacDeployOptions GetDacDeployOptions(DacPacOptions options)
	{
		return new DacDeployOptions
		{
			BlockOnPossibleDataLoss = options.BlockOnPossibleDataLoss,
			GenerateSmartDefaults = options.GenerateSmartDefaults,
			LongRunningCommandTimeout = options.LongRunningCommandTimeout,
			DropObjectsNotInSource = options.DropObjectsNotInSource,
			VerifyDeployment = options.VerifyDeployment
		};
	}
}

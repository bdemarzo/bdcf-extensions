using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Dac;

namespace Bdcf.Extensions.DacPac.Tests;

public class DacPacServiceTests
{
	private class TestableDacPacService : DacPacService
	{
		public TestableDacPacService(IOptions<DacPacOptions> options, ILogger<DacPacService> logger)
			: base(options, logger)
		{
		}

		public new static string? GetDatabaseNameFromConnectionString(string connectionString)
		{
			return DacPacService.GetDatabaseNameFromConnectionString(connectionString);
		}

		public new static Stream GetDacPacStream(string? assemblyName, string dacPacName)
		{
			return DacPacService.GetDacPacStream(assemblyName, dacPacName);
		}

		public new static DacDeployOptions GetDacDeployOptions(DacPacOptions options)
		{
			return DacPacService.GetDacDeployOptions(options);
		}
	}

	[Fact]
	public void GetDatabaseNameFromConnectionString_ValidInitialCatalog_ReturnsDatabaseName()
	{
		var connectionString = "Server=myServerAddress;Initial Catalog=myDataBase;User Id=myUsername;Password=myPassword;";
		var result = TestableDacPacService.GetDatabaseNameFromConnectionString(connectionString);

		Assert.Equal("myDataBase", result);
	}

	[Fact]
	public void GetDatabaseNameFromConnectionString_ValidDatabase_ReturnsDatabaseName()
	{
		string connectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
		var result = TestableDacPacService.GetDatabaseNameFromConnectionString(connectionString);
		if (result == null)
			Assert.Fail("Unable to extract database name.");

		Assert.Equal("myDataBase", result);
	}

	[Fact]
	public void GetDatabaseNameFromConnectionString_MissingDatabaseNameReturnsNull()
	{
		string connectionString = "Server=myServerAddress;User Id=myUsername;Password=myPassword;";
		var databaseName = TestableDacPacService.GetDatabaseNameFromConnectionString(connectionString);

		Assert.Null(databaseName);
	}

	[Fact]
	public void GetDatabaseNameFromConnectionString_InvalidConnectionStringReturnsNull()
	{
		string connectionString = "lorem ipsum";

		Assert.Throws<ArgumentException>(() => TestableDacPacService.GetDatabaseNameFromConnectionString(connectionString));
	}

	[Fact]
	public void GetDacDeployOptions_ReturnsCorrectOptions()
	{
		var options = new DacPacOptions
		{
			ConnectionString = string.Empty,
			DacPacName = string.Empty,
			BlockOnPossibleDataLoss = false,
			GenerateSmartDefaults = true,
			LongRunningCommandTimeout = 120,
			DropObjectsNotInSource = true,
			VerifyDeployment = false
		};

		var result = TestableDacPacService.GetDacDeployOptions(options);

		Assert.Equal(options.BlockOnPossibleDataLoss, result.BlockOnPossibleDataLoss);
		Assert.Equal(options.GenerateSmartDefaults, result.GenerateSmartDefaults);
		Assert.Equal(options.LongRunningCommandTimeout, result.LongRunningCommandTimeout);
		Assert.Equal(options.DropObjectsNotInSource, result.DropObjectsNotInSource);
		Assert.Equal(options.VerifyDeployment, result.VerifyDeployment);
	}

	[Fact]
	public void ApplyDacPac_AppliesDacPacToInMemoryDatabase()
	{
		string databaseName = $"UnitTestDB_{Guid.NewGuid()}";
		string connectionString = $@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog={databaseName}";

		var options = Options.Create(new DacPacOptions
		{
			ConnectionString = connectionString,
			AssemblyName = "Bdcf.Extensions.DacPac.Tests",
			DacPacName = "Bdcf.Extensions.DacPac.Tests.Test.dacpac",
			BlockOnPossibleDataLoss = false,
			GenerateSmartDefaults = true,
			DropObjectsNotInSource = false,
			VerifyDeployment = true
		});

		var mockLogger = new Moq.Mock<ILogger<DacPacService>>();
		var logger = mockLogger.Object;
		var service = new DacPacService(options, logger);

		// Create the empty LocalDB database
		using (var localdbConnection = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog=master"))
		{
			localdbConnection.Open();
			using var command = localdbConnection.CreateCommand();
			command.CommandText = $"CREATE DATABASE [{databaseName}]";
			command.ExecuteNonQuery();
		}

		try
		{
			service.ApplyDacPac();

			// Assert
			using var connection = new SqlConnection(connectionString);
			connection.Open();

			using var command = connection.CreateCommand();

			command.CommandText = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'Tests'";
			int tableCount = (int)command.ExecuteScalar();
			Assert.Equal(1, tableCount);

			command.CommandText = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'Tests' AND COLUMN_NAME = 'testid'";
			int columnCount = (int)command.ExecuteScalar();
			Assert.Equal(1, columnCount);
		}
		finally
		{
			// Clean up - Drop the test database
			using var masterConnection = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog=master");
			masterConnection.Open();

			using var command = masterConnection.CreateCommand();
			command.CommandText = $@"
                        ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        DROP DATABASE [{databaseName}];";
			command.ExecuteNonQuery();
		}
	}
}

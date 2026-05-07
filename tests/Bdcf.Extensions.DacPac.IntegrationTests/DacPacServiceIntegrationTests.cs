using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit.Sdk;

namespace Bdcf.Extensions.DacPac.IntegrationTests;

[Collection("DacPac integration")]
public class DacPacServiceIntegrationTests
{
	private const string LocalDbMasterConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog=master;Connect Timeout=5";
	private const string TestDacPacAssemblyName = "Bdcf.Extensions.DacPac.IntegrationTests";
	private const string TestDacPacResourceName = "Bdcf.Extensions.DacPac.IntegrationTests.Test.dacpac";

	[Fact]
	public void ApplyDacPac_AppliesDacPacToLocalDbDatabase()
	{
		LocalDbTest.SkipIfUnavailable();

		string databaseName = NewDatabaseName();
		string connectionString = GetDatabaseConnectionString(databaseName);
		CreateDatabase(databaseName);

		try
		{
			var service = CreateService(connectionString);

			service.ApplyDacPac();

			AssertTestTableExists(connectionString);
			AssertTestIdColumnExists(connectionString);
		}
		finally
		{
			DropDatabaseIfExists(databaseName);
		}
	}

	[Fact]
	public void ApplyDacPac_UsesNamedConnectionString_WhenSpecifiedAndExists()
	{
		LocalDbTest.SkipIfUnavailable();

		string databaseName = NewDatabaseName();
		string namedConnectionString = GetDatabaseConnectionString(databaseName);
		string otherConnectionString = GetDatabaseConnectionString($"OtherDb_{Guid.NewGuid():N}");
		string name = $"UnitTest_NamedConn_{Guid.NewGuid():N}";

		RemoveNamedConnectionString(name);
		CreateDatabase(databaseName);

		try
		{
			AddNamedConnectionString(name, namedConnectionString);

			var service = CreateService(
				otherConnectionString,
				connectionStringName: name,
				dacPacAssembly: typeof(DacPacServiceIntegrationTests).Assembly);

			service.ApplyDacPac();

			AssertTestTableExists(namedConnectionString);
		}
		finally
		{
			RemoveNamedConnectionString(name);
			DropDatabaseIfExists(databaseName);
		}
	}

	[Fact]
	public void ApplyDacPac_UsesOptionsConnectionString_WhenNamedExistsButEmpty()
	{
		LocalDbTest.SkipIfUnavailable();

		string databaseName = NewDatabaseName();
		string optionsConnectionString = GetDatabaseConnectionString(databaseName);
		string name = $"UnitTest_NamedConn_Empty_{Guid.NewGuid():N}";

		RemoveNamedConnectionString(name);
		CreateDatabase(databaseName);

		try
		{
			AddNamedConnectionString(name, string.Empty);

			var service = CreateService(
				optionsConnectionString,
				connectionStringName: name,
				dacPacResourceMarkerType: typeof(DacPacServiceIntegrationTests));

			service.ApplyDacPac();

			AssertTestTableExists(optionsConnectionString);
		}
		finally
		{
			RemoveNamedConnectionString(name);
			DropDatabaseIfExists(databaseName);
		}
	}

	private static DacPacService CreateService(
		string connectionString,
		string connectionStringName = "",
		Assembly? dacPacAssembly = null,
		Type? dacPacResourceMarkerType = null)
	{
		var options = Options.Create(new DacPacOptions
		{
			ConnectionString = connectionString,
			ConnectionStringName = connectionStringName,
			AssemblyName = dacPacAssembly is null && dacPacResourceMarkerType is null ? TestDacPacAssemblyName : null,
			DacPacAssembly = dacPacAssembly,
			DacPacResourceMarkerType = dacPacResourceMarkerType,
			DacPacName = TestDacPacResourceName,
			BlockOnPossibleDataLoss = false,
			GenerateSmartDefaults = true,
			DropObjectsNotInSource = false,
			VerifyDeployment = true
		});

		return new DacPacService(options, NullLogger<DacPacService>.Instance);
	}

	private static string NewDatabaseName() => $"UnitTestDB_{Guid.NewGuid():N}";

	private static string GetDatabaseConnectionString(string databaseName)
	{
		return $@"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Initial Catalog={databaseName};Connect Timeout=5";
	}

	private static void CreateDatabase(string databaseName)
	{
		using var connection = new SqlConnection(LocalDbMasterConnectionString);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = $"CREATE DATABASE [{databaseName}]";
		command.ExecuteNonQuery();
	}

	private static void DropDatabaseIfExists(string databaseName)
	{
		try
		{
			using var connection = new SqlConnection(LocalDbMasterConnectionString);
			connection.Open();

			using var command = connection.CreateCommand();
			command.CommandText = $@"
ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [{databaseName}];";
			command.ExecuteNonQuery();
		}
		catch (SqlException)
		{
		}
	}

	private static void AssertTestTableExists(string connectionString)
	{
		using var connection = new SqlConnection(connectionString);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Tests'";

		int tableCount = (int)command.ExecuteScalar();
		Assert.Equal(1, tableCount);
	}

	private static void AssertTestIdColumnExists(string connectionString)
	{
		using var connection = new SqlConnection(connectionString);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Tests' AND COLUMN_NAME = 'testid'";

		int columnCount = (int)command.ExecuteScalar();
		Assert.Equal(1, columnCount);
	}

	private static void AddNamedConnectionString(string name, string connectionString)
	{
		var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		config.ConnectionStrings.ConnectionStrings.Remove(name);
		config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString));
		config.Save(ConfigurationSaveMode.Modified);
		ConfigurationManager.RefreshSection("connectionStrings");
	}

	private static void RemoveNamedConnectionString(string name)
	{
		var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		config.ConnectionStrings.ConnectionStrings.Remove(name);
		config.Save(ConfigurationSaveMode.Modified);
		ConfigurationManager.RefreshSection("connectionStrings");
	}

	private static class LocalDbTest
	{
		public static void SkipIfUnavailable()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				throw SkipException.ForSkip("SQL Server LocalDB is only available on Windows.");
			}

			try
			{
				using var connection = new SqlConnection(LocalDbMasterConnectionString);
				connection.Open();
			}
			catch (Exception ex) when (ex is SqlException or InvalidOperationException)
			{
				throw SkipException.ForSkip($"SQL Server LocalDB is unavailable: {ex.Message}");
			}
		}
	}
}

[CollectionDefinition("DacPac integration", DisableParallelization = true)]
public class DacPacIntegrationCollection
{
}

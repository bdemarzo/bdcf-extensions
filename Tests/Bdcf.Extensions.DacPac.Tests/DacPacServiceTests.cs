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
	public void GetDatabaseNameFromConnectionString_InvalidConnectionStringThrowsArgumentException()
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
	public async Task ApplyDacPacAsync_CanceledBeforeStart_ThrowsOperationCanceledException()
	{
		var options = Options.Create(new DacPacOptions
		{
			ConnectionString = string.Empty,
			DacPacName = string.Empty
		});
		var service = new DacPacService(options, Moq.Mock.Of<ILogger<DacPacService>>());
		using var cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();

		await Assert.ThrowsAsync<OperationCanceledException>(() => service.ApplyDacPacAsync(cancellationTokenSource.Token));
	}
}

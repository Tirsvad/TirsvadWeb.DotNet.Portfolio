//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;

//using Portfolio.Infrastructure.Persistents;
//using Portfolio.Infrastructure.Services;

//using System.Collections;
//using System.Data.Common;
//using System.Diagnostics;

//namespace Portfolio.Infrastructure.Tests.Persistents;

//[TestClass]
//public class ApplicationDbContextTests
//{
//    private readonly DbContextOptionsServices? _dbContextOptionsServices;

//    public ApplicationDbContextTests()
//    {
//        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
//        {
//            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
//        }
//        //if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DB_PORTFOLIO_DBNAME")))
//        //{
//        //    Environment.SetEnvironmentVariable("DB_PORTFOLIO_DBNAME", "TirsvadWebPortfolio_Test");
//        //}

//#if DEBUG
//        string[] strings = [
//            "ASPNETCORE_ENVIRONMENT",
//            "DB_PORTFOLIO_DBHOST",
//            "DB_PORTFOLIO_DBNAME",
//            "DB_PORTFOLIO_DBUSER",
//            "DB_PORTFOLIO_DBPASSWORD"
//        ];
//        foreach (DictionaryEntry e in Environment.GetEnvironmentVariables())
//            if (strings.Contains(e.Key?.ToString() ?? string.Empty) && e.Value != null)
//                Debug.WriteLine($"{e.Key}={e.Value}");
//#endif
//        _dbContextOptionsServices = new DbContextOptionsServices();
//    }

//    [TestMethod]
//    [DataRow("Development")]
//    [DataRow("Release")]
//    [DataRow("Test")]
//    [TestCategory("Functional")]
//    public async Task ConnectionString_IsSet_For_Environment(string env)
//    {
//        // Ensure CreateOptions reads the intended environment value used by the test
//        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);

//        DbContextOptions<ApplicationDbContext> options = _dbContextOptionsServices!.CreateOptions(env);

//        // This is a connection-string-only functional test — do not reach out to a docker-hosted
//        // SQL Server because the test runner (Test Explorer) may execute inside a container
//        // where 127.0.0.1 is not mapped to the host. Removing the integration DB reachability
//        // check keeps this test deterministic in CI/local developer scenarios.
//        // await EnsureIntegrationDatabaseAvailableAsync(_dbContextOptionsServices, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

//        using ApplicationDbContext ctx = new(options);
//        // Avoid performing an actual database migration during a connection-string-only unit test.
//        // Migration requires a reachable SQL Server with matching credentials and causes brittle failures
//        // when running in environments without the test database. The intent of this test is to
//        // validate the resolved connection string, not to modify the database schema.
//        //await ctx.Database.EnsureCreatedAsync(TestContext.CancellationToken);
//        //ctx.Database.MigrateAsync(TestContext.CancellationToken).GetAwaiter().GetResult();

//        string conn = ctx.Database.GetDbConnection().ConnectionString ?? string.Empty;

//        // Validate that the connection string is not empty and is a valid SQL Server connection string
//        Assert.IsFalse(string.IsNullOrWhiteSpace(conn), $"Expected non-empty connection string for env '{env}'");
//        try
//        {
//            SqlConnectionStringBuilder sb = new(conn);
//            Assert.IsFalse(string.IsNullOrWhiteSpace(sb.DataSource), $"Connection string missing Data Source for env '{env}'");
//            Assert.IsFalse(string.IsNullOrWhiteSpace(sb.InitialCatalog), $"Connection string missing Initial Catalog (database) for env '{env}'");
//        }
//        catch (Exception ex)
//        {
//            Assert.Fail($"Connection string for env '{env}' is not a valid SQL Server connection string: {ex.Message}");
//        }
//    }

//    [TestMethod]
//    [DataRow("Development")]
//    [DataRow("Release")]
//    [DataRow("Test")]
//    [TestCategory("Integration")]
//    public async Task ConnectionString_CanOpenAndQueryDatabase_For_Environment(string env)
//    {
//        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", env);

//        //await EnsureIntegrationDatabaseAvailableAsync(_dbContextOptionsServices!, env);

//        DbContextOptions<ApplicationDbContext> options = _dbContextOptionsServices!.CreateOptions(env) ?? throw new InvalidOperationException("Failed to create DbContextOptions");

//        using ApplicationDbContext ctx = new(options);

//        DbConnection conn = ctx.Database.GetDbConnection();
//        Debug.WriteLine($"ConnectionString = {conn.ConnectionString}");
//        try
//        {
//            await conn.OpenAsync(TestContext.CancellationToken);

//            using DbCommand cmd = conn.CreateCommand();
//            cmd.CommandText = "SELECT 1";
//            object? result = await cmd.ExecuteScalarAsync(TestContext.CancellationToken);

//            Assert.IsNotNull(result, $"Expected a result when querying DB for env '{env}'");

//            int intResult = Convert.ToInt32(result);
//            Assert.AreEqual(1, intResult, $"Expected query to return 1 for env '{env}'");
//        }
//        finally
//        {
//            try { conn.Close(); } catch { }
//        }
//    }

//    public TestContext TestContext { get; set; }
//}

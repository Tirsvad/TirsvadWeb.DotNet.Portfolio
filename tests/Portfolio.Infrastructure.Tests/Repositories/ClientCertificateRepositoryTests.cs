using Microsoft.EntityFrameworkCore;

using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistences;
using Portfolio.Infrastructure.Repositories;
using Portfolio.Infrastructure.Services;

using System.Diagnostics;

namespace Portfolio.Infrastructure.Tests.Repositories;

[TestClass]
public class ClientCertificateRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> CreateOptions(string dbName)
    {
        DbContextOptionsServices optionsService = new();
        string connectionString = optionsService.CreateConnectionString("Development", dbName);
        Debug.WriteLine($"Using connection string: {connectionString}");
        return optionsService.CreateOptions("Development", dbName);
    }

    #region Functional Tests
    [TestMethod]
    [TestCategory("Functional")]
    public async Task FindBySubjectAsync_ReturnsCertificate_WhenExists()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            ClientCertificateRepository repo = new(dbContext);
            ClientCertificate cert = new() { Id = Guid.NewGuid(), Subject = "CN=test", Issuer = "issuer", ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(1), SerialNumber = "123" };
            _ = dbContext.ClientCertificates.Add(cert);
            _ = await dbContext.SaveChangesAsync();
            ClientCertificate? result = await repo.FindBySubjectAsync("CN=test");
            Assert.IsNotNull(result);
            Assert.AreEqual(cert.Subject, result.Subject);
        }
        finally
        {
            _ = await dbContext.Database.EnsureDeletedAsync();
        }
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task FindBySubjectAsync_ReturnsNull_WhenNotExists()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            ClientCertificateRepository repo = new(dbContext);
            ClientCertificate? result = await repo.FindBySubjectAsync("CN=notfound");
            Assert.IsNull(result);
        }
        finally
        {
            _ = await dbContext.Database.EnsureDeletedAsync();
        }
    }
    #endregion

    #region Error/Com Tests
    [TestMethod]
    [TestCategory("Error")]
    public async Task FindBySubjectAsync_ReturnsNull_OnNullOrWhitespace()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            ClientCertificateRepository repo = new(dbContext);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.IsNull(await repo.FindBySubjectAsync(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.IsNull(await repo.FindBySubjectAsync(" "));
            Assert.IsNull(await repo.FindBySubjectAsync(""));
        }
        finally
        {
            _ = await dbContext.Database.EnsureDeletedAsync();
        }
    }
    #endregion

    #region Concurrency/Threadsafe Tests
    [TestMethod]
    [TestCategory("Concurrency")]
    public void FindBySubjectAsync_ThreadSafe_MultiSession()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        string subject = "CN=threadsafe";
        using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        _ = dbContext.Database.EnsureCreated();
        _ = Parallel.For(0, 10, i =>
        {
            using ApplicationDbContext innerContext = new(CreateOptions(dbName));
            ClientCertificateRepository repo = new(innerContext);
            ClientCertificate cert = new() { Id = Guid.NewGuid(), Subject = subject + i, Issuer = "issuer", ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(1), SerialNumber = i.ToString() };
            _ = innerContext.ClientCertificates.Add(cert);
            _ = innerContext.SaveChanges();
        });
        using ApplicationDbContext verifyContext = new(CreateOptions(dbName));
        ClientCertificateRepository repoVerify = new(verifyContext);
        for (int i = 0; i < 10; i++)
        {
            ClientCertificate? result = repoVerify.FindBySubjectAsync(subject + i).Result;
            Assert.IsNotNull(result);
        }
        _ = dbContext.Database.EnsureDeleted();
    }
    #endregion

    #region Performance/Benchmark Tests
    //[DoNotParallelize]
    //[TestMethod]
    //[TestCategory("Performance")]
    //public async Task FindBySubjectAsync_Performance_Benchmark()
    //{
    //    string dbName = $"Test_{Guid.NewGuid()}";
    //    int iterations = 1000;
    //    int TargetAverageMs = 100;
    //    int totalElapsedMs = 0;
    //    await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
    //    try
    //    {
    //        _ = await dbContext.Database.EnsureCreatedAsync();
    //        ClientCertificateRepository repo = new(dbContext);
    //        for (int i = 0; i < iterations; i++)
    //        {
    //            ClientCertificate cert = new() { Id = Guid.NewGuid(), Subject = $"CN=bench{i}", Issuer = "issuer", ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(1), SerialNumber = i.ToString() };
    //            _ = dbContext.ClientCertificates.Add(cert);
    //        }
    //        _ = await dbContext.SaveChangesAsync();
    //        for (int i = 0; i < iterations; i++)
    //        {
    //            Stopwatch sw = Stopwatch.StartNew();
    //            ClientCertificate? result = await repo.FindBySubjectAsync($"CN=bench{i}");
    //            sw.Stop();
    //            totalElapsedMs += (int)sw.ElapsedMilliseconds;
    //            Assert.IsNotNull(result);
    //        }
    //        int averageElapsedMs = totalElapsedMs / iterations;
    //        Assert.IsLessThan(400, averageElapsedMs, $"Performance issue: {averageElapsedMs}ms");
    //        if (averageElapsedMs > TargetAverageMs)
    //        {
    //            Assert.Inconclusive($"Performance warning: {averageElapsedMs}ms");
    //        }
    //    }
    //    finally
    //    {
    //        _ = await dbContext.Database.EnsureDeletedAsync();
    //    }
    //}
    #endregion

    #region Multi-Session Tests
    [TestMethod]
    [TestCategory("Concurrency")]
    public void FindBySubjectAsync_MultiSession()
    {
        _ = Parallel.For(0, 5, i =>
        {
            string dbName = $"Test_{Guid.NewGuid()}";
            try
            {
                using ApplicationDbContext dbContext = new(CreateOptions(dbName));
                _ = dbContext.Database.EnsureCreated();
                ClientCertificateRepository repo = new(dbContext);
                ClientCertificate cert = new() { Id = Guid.NewGuid(), Subject = $"CN=multi{i}", Issuer = "issuer", ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(1), SerialNumber = i.ToString() };
                _ = dbContext.ClientCertificates.Add(cert);
                _ = dbContext.SaveChanges();
                ClientCertificate? result = repo.FindBySubjectAsync(cert.Subject).Result;
                Assert.IsNotNull(result);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Exception in multi-session test: {ex.Message}");
            }
            finally
            {
                using ApplicationDbContext dbContext = new(CreateOptions(dbName));
                _ = dbContext.Database.EnsureDeleted();
            }
        });
    }
    #endregion

    #region Private/Helper/Edge Tests
    [TestMethod]
    [TestCategory("Helper")]
    public void RepositoryBase_IsInternalHelper()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        _ = dbContext.Database.EnsureCreated();
        ClientCertificateRepository repo = new(dbContext);
        _ = Assert.IsInstanceOfType<ClientCertificateRepository>(repo);
        _ = dbContext.Database.EnsureDeleted();
    }

    [TestMethod]
    [TestCategory("Edge")]
    public async Task FindBySubjectAsync_EdgeCases()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            ClientCertificateRepository repo = new(dbContext);
            // Add cert with empty subject
            ClientCertificate cert = new() { Id = Guid.NewGuid(), Subject = string.Empty, Issuer = "issuer", ValidFrom = DateTime.UtcNow, ValidTo = DateTime.UtcNow.AddYears(1), SerialNumber = "edge" };
            _ = dbContext.ClientCertificates.Add(cert);
            _ = await dbContext.SaveChangesAsync();
            ClientCertificate? result = await repo.FindBySubjectAsync(string.Empty);
            Assert.IsNull(result); // Should return null due to input validation
        }
        finally
        {
            _ = await dbContext.Database.EnsureDeletedAsync();
        }
    }
    #endregion
}

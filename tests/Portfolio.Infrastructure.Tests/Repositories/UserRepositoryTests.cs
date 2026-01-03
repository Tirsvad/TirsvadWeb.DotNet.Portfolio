using Microsoft.EntityFrameworkCore;

using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistences;
using Portfolio.Infrastructure.Repositories;
using Portfolio.Infrastructure.Services;

using System.Diagnostics;

namespace Portfolio.Infrastructure.Tests.Repositories;

[TestClass]
public class UserRepositoryTests
{

    private DbContextOptions<ApplicationDbContext> CreateOptions(string dbName)
    {
        // Do not use environment variables for thread safety
        DbContextOptionsServices optionsService = new();
        // Pass dbName as the 'name' parameter to override Initial Catalog
        string connectionString = optionsService.CreateConnectionString("Development", dbName);
        Debug.WriteLine($"Using connection string: {connectionString}");
        return optionsService.CreateOptions("Development", dbName);
    }

    #region Functional Tests
    [TestMethod]
    [TestCategory("Functional")]
    public async Task AddAndGetUser_Works()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            UserRepository repo = new(dbContext);
            ApplicationUser user = new() { Id = Guid.NewGuid(), UserName = "testuser" };
            await repo.AddAsync(user);
            ApplicationUser? result = await repo.GetByIdAsync(user.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserName, result.UserName);
        }
        finally
        {
            _ = await dbContext.Database.EnsureDeletedAsync();
        }
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task UpdateUser_Works()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        Debug.WriteLine($"Database Name for functional test: {dbName}");
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            UserRepository repo = new(dbContext);
            ApplicationUser user = new() { Id = Guid.NewGuid(), UserName = "testuser" };
            await repo.AddAsync(user);
            user.UserName = "updateduser";
            await repo.UpdateAsync(user);
            ApplicationUser? result = await repo.GetByIdAsync(user.Id);
            Assert.IsNotNull(result);
            Assert.AreEqual("updateduser", result!.UserName);
        }
        finally
        {
            _ = await dbContext.Database.EnsureDeletedAsync();
        }
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task DeleteUser_Works()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        Debug.WriteLine($"Database Name for functional test: {dbName}");
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            UserRepository repo = new(dbContext);
            ApplicationUser user = new() { Id = Guid.NewGuid(), UserName = "testuser" };
            await repo.AddAsync(user);
            await repo.DeleteAsync(user);
            ApplicationUser? result = await repo.GetByIdAsync(user.Id);
            Assert.IsNull(result);
        }
        finally
        {
            _ = await dbContext.Database.EnsureDeletedAsync();
        }
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetAllUsers_Works()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        Debug.WriteLine($"Database Name for functional test: {dbName}");
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            UserRepository repo = new(dbContext);
            await repo.AddAsync(new ApplicationUser { Id = Guid.NewGuid(), UserName = "user1" });
            await repo.AddAsync(new ApplicationUser { Id = Guid.NewGuid(), UserName = "user2" });
            IEnumerable<ApplicationUser> users = await repo.GetAllAsync();
            Assert.AreEqual(2, users.Count());
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
    public async Task AddAsync_ThrowsOnNull()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        Debug.WriteLine($"Database Name for error test: {dbName}");
        await using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        try
        {
            _ = await dbContext.Database.EnsureCreatedAsync();
            UserRepository repo = new(dbContext);
            try
            {
                await repo.AddAsync(null!);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                // Test passes
            }
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
    public void AddAsync_ThreadSafe_MultiSession()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        Debug.WriteLine($"Database Name for concurrency test: {dbName}");
        using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        _ = dbContext.Database.EnsureCreated();
        _ = Parallel.For(0, 10, i =>
        {
            using ApplicationDbContext innerContext = new(CreateOptions(dbName));
            UserRepository repo = new(innerContext);
            ApplicationUser user = new() { Id = Guid.NewGuid(), UserName = $"user{i}" };
            repo.AddAsync(user).Wait();
        });
        using ApplicationDbContext verifyContext = new(CreateOptions(dbName));
        UserRepository verifyRepo = new(verifyContext);
        IEnumerable<ApplicationUser> users = verifyRepo.GetAllAsync().Result;
        Assert.AreEqual(10, users.Count());
        _ = dbContext.Database.EnsureDeleted();
    }
    #endregion

    #region Performance/Benchmark Tests
    //[TestMethod]
    //[TestCategory("Performance")]
    //public async Task AddAsync_Performance_Benchmark()
    //{
    //    string dbName = $"Test_{Guid.NewGuid()}";
    //    await using var dbContext = new ApplicationDbContext(CreateOptions(dbName));
    //    await dbContext.Database.EnsureCreatedAsync();
    //    UserRepository repo = new(dbContext);
    //    int iterations = 1000;
    //    int TargetAverageMs = 100; // in milliseconds
    //    int totalElapsedMs = 0;
    //    for (int i = 0; i < iterations; i++)
    //    {
    //        ApplicationUser user = new() { Id = Guid.NewGuid(), UserName = $"benchuser{i}" };
    //        Stopwatch sw = Stopwatch.StartNew();
    //        await repo.AddAsync(user);
    //        sw.Stop();
    //        totalElapsedMs += (int)sw.ElapsedMilliseconds;
    //    }
    //    int averageElapsedMs = totalElapsedMs / iterations;
    //    Assert.IsLessThan(400, averageElapsedMs, $"Performance issue: {averageElapsedMs}ms");
    //    if ((totalElapsedMs / iterations) > TargetAverageMs)
    //    {
    //        Assert.Inconclusive($"Performance warning: {averageElapsedMs}ms");
    //    }
    //    await dbContext.Database.EnsureDeletedAsync();
    //}
    #endregion

    #region Multi-Session Tests
    [TestMethod]
    [TestCategory("Concurrency")]
    public void AddAsync_MultiSession()
    {
        string dbName = $"Test_{Guid.NewGuid()}";
        using ApplicationDbContext dbContext = new(CreateOptions(dbName));
        _ = dbContext.Database.EnsureCreated();
        _ = Parallel.For(0, 5, i =>
        {
            using ApplicationDbContext innerContext = new(CreateOptions(dbName));
            UserRepository repo = new(innerContext);
            ApplicationUser user = new() { Id = Guid.NewGuid(), UserName = $"user{i}" };
            repo.AddAsync(user).Wait();
            ApplicationUser? result = repo.GetByIdAsync(user.Id).Result;
            Assert.IsNotNull(result);
        });
        _ = dbContext.Database.EnsureDeleted();
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
        UserRepository repo = new(dbContext);
        _ = Assert.IsInstanceOfType<UserRepository>(repo);
        _ = dbContext.Database.EnsureDeleted();
    }
    #endregion
}


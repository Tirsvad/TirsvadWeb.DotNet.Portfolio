using Microsoft.EntityFrameworkCore;

using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistences.Configurations;

using System.Diagnostics;

namespace Portfolio.Infrastructure.Tests.Persistents.Configurations;

[TestClass]
public class ApplicationUserConfigurationTests
{
    // Functional Test: Ensure configuration applies without error
    [TestMethod]
    public void Configure_ShouldApplyConfigurationWithoutError()
    {
        // Arrange
        ModelBuilder modelBuilder = new ModelBuilder();
        ApplicationUserConfiguration config = new ApplicationUserConfiguration();
        // Act
        config.Configure(modelBuilder.Entity<ApplicationUser>());
        // Assert (implicit: no exception thrown)
    }

    // COM Error Test: Simulate COM error (not directly applicable, but test for exception safety)
    [TestMethod]
    public void Configure_ShouldNotThrowOnComError()
    {
        // Arrange
        ModelBuilder modelBuilder = new ModelBuilder();
        ApplicationUserConfiguration config = new ApplicationUserConfiguration();
        // Act & Assert
        try
        {
            config.Configure(modelBuilder.Entity<ApplicationUser>());
        }
        catch (Exception ex)
        {
            Assert.Fail($"Unexpected exception: {ex.Message}");
        }
    }

    // Concurrency Test: Thread safety
    [TestMethod]
    public void Configure_ShouldBeThreadSafe()
    {
        // Arrange
        ApplicationUserConfiguration config = new ApplicationUserConfiguration();
        // Act & Assert
        Parallel.For(0, 10, i =>
        {
            ModelBuilder modelBuilder = new ModelBuilder();
            config.Configure(modelBuilder.Entity<ApplicationUser>());
        });
    }

    // Performance Test: Benchmark configuration
    [TestMethod]
    public void Configure_PerformanceBenchmark()
    {
        // Arrange
        ApplicationUserConfiguration config = new ApplicationUserConfiguration();
        int iterations = 1000;
        int timeLimitMilliseconds = (int)(iterations * 1.5);

        // Act
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            ModelBuilder modelBuilder = new ModelBuilder();
            config.Configure(modelBuilder.Entity<ApplicationUser>());
        }
        sw.Stop();

        // Assert
        Assert.IsLessThan(timeLimitMilliseconds, sw.ElapsedMilliseconds, $"Configuration took too long: {sw.ElapsedMilliseconds}ms");
    }

    // Multi-session Test: Multiple independent configurations
    [TestMethod]
    public void Configure_MultiSession()
    {
        // Arrange
        ApplicationUserConfiguration config1 = new ApplicationUserConfiguration();
        ApplicationUserConfiguration config2 = new ApplicationUserConfiguration();
        ModelBuilder modelBuilder1 = new ModelBuilder();
        ModelBuilder modelBuilder2 = new ModelBuilder();
        // Act
        config1.Configure(modelBuilder1.Entity<ApplicationUser>());
        config2.Configure(modelBuilder2.Entity<ApplicationUser>());
        // Assert (implicit: no exception thrown)
    }

    // Private/Edge/Helper: Not applicable, as there are no private or helper methods in this configuration class
}

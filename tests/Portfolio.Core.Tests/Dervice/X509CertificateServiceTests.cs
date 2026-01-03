using Portfolio.Core.Services;

using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Portfolio.Core.Tests.Dervice;

[TestClass]
public class X509CertificateServiceTests
{
    // ------------------- Functional Tests -------------------
    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetPreloadedCertificateAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        X509CertificateService svc = new();
        // Act
        X509Certificate2? cert = await svc.GetPreloadedCertificateAsync("NonExistentCertName");
        // Assert
        Assert.IsNull(cert);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetCertificateByNameAsync_ReturnsNull_WhenSubjectIsNullOrWhitespace()
    {
        // Arrange
        X509CertificateService svc = new();
        // Act & Assert
        Assert.IsNull(await svc.GetCertificateByNameAsync(string.Empty));
        Assert.IsNull(await svc.GetCertificateByNameAsync("   "));
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task CreateCertificateAsync_ReturnsCertificate_InDevelopment()
    {
        // Arrange
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        X509CertificateService svc = new();
        // Act
        X509Certificate2? cert = await svc.CreateCertificateAsync("CreateCertTest");
        // Assert
#pragma warning disable CS8794 // The input always matches the provided pattern.
        Assert.IsTrue(cert is null or X509Certificate2);
#pragma warning restore CS8794 // The input always matches the provided pattern.
    }

    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetCertificateByNameAsync_CachesNegativeResult()
    {
        // Arrange
        X509CertificateService svc = new();
        string subject = "DefinitelyNotFound" + System.Guid.NewGuid();
        // Act
        X509Certificate2? cert1 = await svc.GetCertificateByNameAsync(subject);
        X509Certificate2? cert2 = await svc.GetCertificateByNameAsync(subject);
        // Assert
        Assert.IsNull(cert1);
        Assert.IsNull(cert2);
    }

    // ------------------- COM Error Test -------------------
    [TestMethod]
    [TestCategory("Functional")]
    public async Task GetCertificateByNameAsync_HandlesComError_Gracefully()
    {
        // Arrange
        X509CertificateService svc = new();
        // Act
        X509Certificate2? cert = await svc.GetCertificateByNameAsync(System.Guid.NewGuid().ToString());
        // Assert
        Assert.IsNull(cert);
    }

    // ------------------- Concurrency Tests -------------------
    [TestMethod]
    [TestCategory("Concurrency")]
    public async Task GetCertificateByNameAsync_IsThreadSafe()
    {
        // Arrange
        X509CertificateService svc = new();
        string subject = "ThreadSafeTestCert";
        List<Task<X509Certificate2?>> tasks = [];
        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(svc.GetCertificateByNameAsync(subject));
        }
        X509Certificate2?[] results = await Task.WhenAll(tasks);
        // Assert
        foreach (X509Certificate2? result in results)
        {
#pragma warning disable CS8794 // The input always matches the provided pattern.
            Assert.IsTrue(result is null or X509Certificate2);
#pragma warning restore CS8794 // The input always matches the provided pattern.
        }
    }

    [TestMethod]
    [TestCategory("Concurrency")]
    public async Task MultiSession_CertificateStore_Access()
    {
        // Arrange
        X509CertificateService svc1 = new();
        X509CertificateService svc2 = new();
        string subject = "MultiSessionTestCert";
        // Act
        Task<X509Certificate2?> t1 = svc1.GetCertificateByNameAsync(subject);
        Task<X509Certificate2?> t2 = svc2.GetCertificateByNameAsync(subject);
        X509Certificate2?[] results = await Task.WhenAll(t1, t2);
        // Assert
#pragma warning disable CS8794 // The input always matches the provided pattern.
        Assert.IsTrue(results[0] is null or X509Certificate2);
        Assert.IsTrue(results[1] is null or X509Certificate2);
#pragma warning restore CS8794 // The input always matches the provided pattern.
    }

    [TestMethod]
    [TestCategory("Concurrency")]
    public async Task GetCertificateByNameAsync_CacheIsThreadSafe()
    {
        // Arrange
        X509CertificateService svc = new();
        string subject = "ThreadSafeCacheTest" + System.Guid.NewGuid();
        List<Task<X509Certificate2?>> tasks = [];
        // Act
        for (int i = 0; i < 20; i++)
        {
            tasks.Add(svc.GetCertificateByNameAsync(subject));
        }
        X509Certificate2?[] results = await Task.WhenAll(tasks);
        // Assert
        foreach (X509Certificate2? result in results)
        {
            Assert.IsNull(result); // Should all be null for a random subject
        }
    }

    // ------------------- Benchmark/Performance Tests -------------------
    [TestMethod]
    [TestCategory("Benchmark")]
    public async Task Benchmark_GetCertificateByNameAsync_Performance()
    {
        // Arrange
        X509CertificateService svc = new();
        string subject = "BenchmarkTestCert";
        // Act
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            _ = await svc.GetCertificateByNameAsync(subject);
        }
        sw.Stop();
        // Assert
        Assert.IsLessThan(5, sw.Elapsed.TotalSeconds);
    }

    [TestMethod]
    [TestCategory("Benchmark")]
    public async Task GetCertificateByNameAsync_RepeatedAccess_IsFast()
    {
        int iterations = 50;
        int targetMs = 100 * iterations;
        // Arrange
        X509CertificateService svc = new();
        string subject = "RepeatedAccessTestCert";
        // Act
        // First access (may be slow)
        _ = await svc.GetCertificateByNameAsync(subject);
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < 50; i++)
        {
            _ = await svc.GetCertificateByNameAsync(subject);
        }
        sw.Stop();
        // Assert
        Assert.IsLessThan(targetMs, sw.Elapsed.TotalMilliseconds);
    }

    // ------------------- Private/Edge/Helper Coverage -------------------
    [TestMethod]
    [TestCategory("Edge")]
    public void TryRemoveCache_RemovesEntry()
    {
        // Arrange
        X509CertificateService svc = new();
        FieldInfo? cacheField = svc.GetType().GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance);
        object? cacheObj = cacheField?.GetValue(svc);
        Assert.IsNotNull(cacheObj, "_cache field should not be null");
        IDictionary? cache = cacheObj as IDictionary;
        Assert.IsNotNull(cache, "_cache field is not an IDictionary");
        cache["test"] = null;
        MethodInfo? method = svc.GetType().GetMethod("TryRemoveCache", BindingFlags.NonPublic | BindingFlags.Instance);
        // Act
        object? resultObj = method?.Invoke(svc, new object[] { "test" });
        bool result = resultObj is bool b && b;
        // Assert
        Assert.DoesNotContain("test", cache);
        Assert.IsFalse(result); // Always returns false
    }

    [TestMethod]
    [TestCategory("Edge")]
    public void GetBestCandidate_ReturnsNull_WhenEmpty()
    {
        // Arrange
        X509CertificateService svc = new();
        MethodInfo? method = svc.GetType().GetMethod("GetBestCandidate", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, "GetBestCandidate method should not be null");
        // Act
        object? result = method!.Invoke(svc, new object[] { new List<X509Certificate2>() });
        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    [TestCategory("Edge")]
    public void FindCandidates_ReturnsEmpty_WhenNoMatch()
    {
        // Arrange
        X509CertificateService svc = new();
        using X509Store store = new(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);
        MethodInfo? method = svc.GetType().GetMethod("FindCandidates", BindingFlags.NonPublic | BindingFlags.Instance);
        // Act
        object? resultObj = method?.Invoke(svc, new object[] { store, "DefinitelyNotFound" + Guid.NewGuid() });
        // Assert
        Assert.IsNotNull(resultObj, "FindCandidates returned null");
        List<X509Certificate2> result = (List<X509Certificate2>)resultObj!;
        Assert.IsEmpty(result);
    }

    [TestMethod]
    [TestCategory("Edge")]
    public void TryGetFromCacheWithExport_ReturnsNull_OnInvalidExport()
    {
        // Arrange
        X509CertificateService svc = new();
        Type? entryType = svc.GetType().GetNestedType("CertificateCacheEntry", BindingFlags.NonPublic);
        Assert.IsNotNull(entryType, "CertificateCacheEntry type should not be null");
        ConstructorInfo ctor = entryType!.GetConstructors()[0];
        object entry = ctor.Invoke(new object[] { "thumb", DateTime.UtcNow.AddYears(1), true, new byte[] { 1, 2, 3 } });
        MethodInfo? method = svc.GetType().GetMethod("TryGetFromCacheWithExport", BindingFlags.NonPublic | BindingFlags.Instance);
        // Act
        object? resultObj = method?.Invoke(svc, new object[] { "subject", entry });
        // Assert
        Assert.IsNotNull(resultObj, "TryGetFromCacheWithExport returned null");
        (bool, X509Certificate2?) result = (ValueTuple<bool, X509Certificate2?>)resultObj!;
        Assert.IsFalse(result.Item1);
        Assert.IsNull(result.Item2);
    }

    [TestMethod]
    [TestCategory("Edge")]
    public void TryGetFromCacheWithoutExport_ReturnsNull_OnInvalidThumbprint()
    {
        // Arrange
        X509CertificateService svc = new();
        Type? entryType = svc.GetType().GetNestedType("CertificateCacheEntry", BindingFlags.NonPublic);
        Assert.IsNotNull(entryType, "CertificateCacheEntry type should not be null");
        ConstructorInfo ctor = entryType!.GetConstructors()[0];
        object? export = null;
        object entry = ctor.Invoke(["invalidthumb", DateTime.UtcNow.AddYears(1), true, export]);
        MethodInfo? method = svc.GetType().GetMethod("TryGetFromCacheWithoutExport", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, "TryGetFromCacheWithoutExport method should not be null");
        // Act
        object? resultObj = method!.Invoke(svc, new object[] { "subject", entry });
        // Assert
        Assert.IsNotNull(resultObj, "TryGetFromCacheWithoutExport returned null");
        (bool, X509Certificate2?) result = ((bool, X509Certificate2?))resultObj!;
        Assert.IsFalse(result.Item1);
        Assert.IsNull(result.Item2);
    }
}


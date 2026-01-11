using Portfolio.Infrastructure.Extensions;

using System.Diagnostics;

namespace Portfolio.Infrastructure.Tests.Extensions;

[TestClass]
public class DbContextOptionsServicesExtensionsTests
{
    #region Functional Tests
    [TestMethod]
    [TestCategory("Functional")]
    public void RemoveAfter_RemovesElementsAfterMatch()
    {
        // Arrange
        int[] arr = { 1, 2, 3, 4, 5 };
        // Act
        int[] result = arr.RemoveAfter(x => x == 3, includeMatch: true);
        // Assert
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public void RemoveAfter_IncludeMatch_RemovesAfterIncludingMatch()
    {
        // Arrange
        int[] arr = { 1, 2, 3, 4, 5 };
        // Act
        int[] result = arr.RemoveAfter(x => x == 3, includeMatch: true);
        // Assert
        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public void RemoveAfter_NoMatch_ReturnsOriginal()
    {
        // Arrange
        int[] arr = { 1, 2, 3 };
        // Act
        int[] result = arr.RemoveAfter(x => x == 99);
        // Assert
        CollectionAssert.AreEqual(arr, result);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public void RemoveBefore_RemovesElementsBeforeMatch()
    {
        // Arrange
        int[] arr = { 1, 2, 3, 4, 5 };
        // Act
        int[] result = arr.RemoveBefore(x => x == 3);
        // Assert
        CollectionAssert.AreEqual(new[] { 4, 5 }, result);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public void RemoveBefore_IncludeMatch_RemovesBeforeExcludingMatch()
    {
        // Arrange
        int[] arr = { 1, 2, 3, 4, 5 };
        // Act
        int[] result = arr.RemoveBefore(x => x == 3, true);
        // Assert
        CollectionAssert.AreEqual(new[] { 3, 4, 5 }, result);
    }

    [TestMethod]
    [TestCategory("Functional")]
    public void RemoveBefore_NoMatch_ReturnsOriginal()
    {
        // Arrange
        int[] arr = { 1, 2, 3 };
        // Act
        int[] result = arr.RemoveBefore(x => x == 99);
        // Assert
        CollectionAssert.AreEqual(arr, result);
    }
    #endregion

    #region Common Error Tests
    [TestMethod]
    [TestCategory("Error")]
    public void RemoveAfter_NullSource_Throws()
    {
        // Arrange
        int[]? arr = null;
        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => DbContextOptionsServicesExtensions.RemoveAfter(arr!, x => x == 1));
    }

    [TestMethod]
    [TestCategory("Error")]
    public void RemoveAfter_NullPredicate_Throws()
    {
        // Arrange
        int[] arr = { 1 };
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => arr.RemoveAfter(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [TestMethod]
    [TestCategory("Error")]
    public void RemoveBefore_NullSource_Throws()
    {
        // Arrange
        int[]? arr = null;
        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => DbContextOptionsServicesExtensions.RemoveBefore(arr!, x => x == 1));
    }

    [TestMethod]
    [TestCategory("Error")]
    public void RemoveBefore_NullPredicate_Throws()
    {
        // Arrange
        int[] arr = { 1 };
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        // Act & Assert
        _ = Assert.Throws<ArgumentNullException>(() => arr.RemoveBefore(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }
    #endregion

    #region Edge/Helper Tests
    [TestMethod]
    [TestCategory("Edge")]
    public void RemoveAfter_EmptyArray_ReturnsEmpty()
    {
        // Arrange
        int[] arr = Array.Empty<int>();
        // Act
        int[] result = arr.RemoveAfter(x => true);
        // Assert
        CollectionAssert.AreEqual(Array.Empty<int>(), result);
    }

    [TestMethod]
    [TestCategory("Edge")]
    public void RemoveBefore_EmptyArray_ReturnsEmpty()
    {
        // Arrange
        int[] arr = Array.Empty<int>();
        // Act
        int[] result = arr.RemoveBefore(x => true);
        // Assert
        CollectionAssert.AreEqual(Array.Empty<int>(), result);
    }
    #endregion

    #region Concurrency Tests
    [TestMethod]
    [TestCategory("Concurrency")]
    public void RemoveAfter_ThreadSafe_MultiSession()
    {
        // Arrange
        int[] arr = Enumerable.Range(1, 1000).ToArray();
        // Act & Assert
        _ = Parallel.For(0, 10, i =>
        {
            int[] result = arr.RemoveAfter(x => x == 500, true);
            Assert.HasCount(500, result);
        });
    }

    [TestMethod]
    [TestCategory("Concurrency")]
    public void RemoveBefore_ThreadSafe_MultiSession()
    {
        // Arrange
        int[] arr = Enumerable.Range(1, 1000).ToArray();
        // Act & Assert
        _ = Parallel.For(0, 10, i =>
        {
            int[] result = arr.RemoveBefore(x => x == 500, true);
            Assert.HasCount(501, result);
        });
    }
    #endregion

    #region Benchmark Tests
    [DoNotParallelize]
    [TestMethod]
    [TestCategory("Performance")]
    public void RemoveAfter_Performance_Benchmark()
    {
        // Arrange
        int[] arr = Enumerable.Range(1, 100000).ToArray();
        // Act
        Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        int[] result = arr.RemoveAfter(x => x == 50000);
        watch.Stop();
        // Assert
        Assert.IsLessThan(100, watch.ElapsedMilliseconds, $"Performance issue: {watch.ElapsedMilliseconds}ms");
    }

    [DoNotParallelize]
    [TestMethod]
    [TestCategory("Performance")]
    public void RemoveBefore_Performance_Benchmark()
    {
        // Arrange
        int[] arr = Enumerable.Range(1, 100000).ToArray();
        // Act
        Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
        int[] result = arr.RemoveBefore(x => x == 50000);
        watch.Stop();
        // Assert
        Assert.IsLessThan(100, watch.ElapsedMilliseconds, $"Performance issue: {watch.ElapsedMilliseconds}ms");
    }
    #endregion
}


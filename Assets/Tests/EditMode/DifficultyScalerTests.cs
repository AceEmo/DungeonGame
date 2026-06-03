using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class DifficultyScalerTests
{
    [TearDown]
    public void TearDown()
    {
        if (GameManager.Instance != null)
        {
            Object.DestroyImmediate(GameManager.Instance.gameObject);
        }
    }

    [Test]
    public void Scale_WithoutGameManager_ShouldReturnBaseStats()
    {
        DifficultyScaler.ScaledStats stats = DifficultyScaler.Scale(100, 4f, 10);

        Assert.AreEqual(100, stats.MaxHealth);
        Assert.AreEqual(4f, stats.Speed);
        Assert.AreEqual(10, stats.Damage);
    }
}

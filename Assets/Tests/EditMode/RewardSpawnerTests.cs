using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class RewardSpawnerTests
{
    [Test]
    public void PickRandomReward_ShouldReturnOneOfProvidedPrefabs()
    {
        GameObject health = new GameObject("Health");
        GameObject gear1 = new GameObject("Gear1");
        GameObject gear2 = new GameObject("Gear2");

        GameObject result = RewardSpawner.PickRandomReward(health, gear1, gear2);

        Assert.That(result == health || result == gear1 || result == gear2);

        Object.DestroyImmediate(health);
        Object.DestroyImmediate(gear1);
        Object.DestroyImmediate(gear2);
    }
}

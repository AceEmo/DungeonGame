using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PlayerStatsTests
{
    private PlayerStats _stats;

    [SetUp]
    public void SetUp()
    {
        _stats = ScriptableObject.CreateInstance<PlayerStats>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_stats);
    }

    [Test]
    public void ResetAll_ShouldSetFieldsToBaseValuesAndClearScrap()
    {
        _stats.scrap = 50;
        _stats.moveSpeed = 20f;

        _stats.ResetAll();

        Assert.AreEqual(_stats.baseMoveSpeed, _stats.moveSpeed);
        Assert.AreEqual(0, _stats.scrap);
    }

    [Test]
    public void AddScrap_ShouldIncreaseScrapAmountAndInvokeEvent()
    {
        bool eventInvoked = false;
        _stats.OnScrapChanged += () => eventInvoked = true;

        _stats.AddScrap(15);

        Assert.AreEqual(15, _stats.scrap);
        Assert.IsTrue(eventInvoked);
    }
}
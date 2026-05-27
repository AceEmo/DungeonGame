using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class BossRageTests
{
    private BossRage _bossRage;
    private BossContext _context;

    [SetUp]
    public void SetUp()
    {
        _bossRage = new BossRage();
        _context = new BossContext
        {
            Health = new BossHealth(100),
            Data = ScriptableObject.CreateInstance<BossData>(),
            CurrentSpeed = 10f,
            CurrentDamage = 10
        };
        _context.Data.rageThreshold = 0.3f;
        _context.Data.rageSpeedMultiplier = 2f;
        _context.Data.rageDamageMultiplier = 2f;
    }

    [Test]
    public void UpdateRage_WhenHpAboveThreshold_ShouldNotTriggerRage()
    {
        _context.Health.TakeDamage(50);

        _bossRage.UpdateRage(_context);

        Assert.IsFalse(_bossRage.IsRaging);
        Assert.AreEqual(10f, _context.CurrentSpeed);
    }

    [Test]
    public void UpdateRage_WhenHpDropsBelowThreshold_ShouldTriggerRageAndApplyMultipliers()
    {
        _context.Health.TakeDamage(80);

        _bossRage.UpdateRage(_context);

        Assert.IsTrue(_bossRage.IsRaging);
        Assert.AreEqual(20f, _context.CurrentSpeed);
        Assert.AreEqual(20, _context.CurrentDamage);
    }

    [Test]
    public void UpdateRage_WhenAlreadyRaging_ShouldNotApplyMultipliersAgain()
    {
        _context.Health.TakeDamage(80);
        _bossRage.UpdateRage(_context);

        _bossRage.UpdateRage(_context);

        Assert.AreEqual(20f, _context.CurrentSpeed);
        Assert.AreEqual(20, _context.CurrentDamage);
    }
}
using NUnit.Framework;

[TestFixture]
public class BossHealthTests
{
    private BossHealth _health;

    [SetUp]
    public void SetUp()
    {
        _health = new BossHealth(100);
    }

    [Test]
    public void TakeDamage_ShouldReduceCurrentHealth()
    {
        _health.TakeDamage(30);

        Assert.AreEqual(0.7f, _health.HealthPercent());
    }

    [Test]
    public void TakeDamage_WhenAmountExceedsHealth_ShouldClampToZero()
    {
        _health.TakeDamage(120);

        Assert.IsTrue(_health.IsDead);
        Assert.AreEqual(0f, _health.HealthPercent());
    }

    [Test]
    public void TakeDamage_WhenAlreadyDead_ShouldDoNothing()
    {
        _health.TakeDamage(100);
        
        _health.TakeDamage(50);

        Assert.AreEqual(0f, _health.HealthPercent());
    }
}
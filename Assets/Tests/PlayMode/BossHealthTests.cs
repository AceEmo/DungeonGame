using NUnit.Framework;

public class BossHealthTests
{
    [Test]
    public void TakeDamageReducesHealthProperly()
    {
        BossHealth health = new BossHealth(100);

        health.TakeDamage(20);

        Assert.AreEqual(80, health.MaxHealth - 20);
        Assert.IsFalse(health.IsDead);
    }

    [Test]
    public void TakeDamageCannotDropHealthBelowZero()
    {
        BossHealth health = new BossHealth(50);

        health.TakeDamage(100);

        Assert.IsTrue(health.IsDead);
        Assert.AreEqual(0f, health.HealthPercent());
    }

    [Test]
    public void HealthPercentReturnsCorrectRatio()
    {
        BossHealth health = new BossHealth(200);

        health.TakeDamage(50);

        Assert.AreEqual(0.75f, health.HealthPercent());
    }
}
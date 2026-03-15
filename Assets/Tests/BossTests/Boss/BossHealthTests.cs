using NUnit.Framework;

public class BossHealthTests
{
    [Test]
    public void TakeDamage_ReducesHealth()
    {
        var health = new BossHealth(20);

        health.TakeDamage(5);

        Assert.AreEqual(15, health.MaxHealth);
    }

    [Test]
    public void TakeDamage_WhenHealthZero_IsDeadBecomesTrue()
    {
        var health = new BossHealth(10);

        health.TakeDamage(10);

        Assert.IsTrue(health.IsDead);
    }

    [Test]
    public void TakeDamage_WhenAlreadyDead_DoesNotChangeHealth()
    {
        var health = new BossHealth(5);

        health.TakeDamage(10);
        health.TakeDamage(5);

        Assert.IsTrue(health.IsDead);
    }
}
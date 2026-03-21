using NUnit.Framework;
using UnityEngine;

public class BossRageTests
{
    private BossContext context;
    private BossData data;
    private GameObject bossObject;
    private SpriteRenderer spriteRenderer;

    [SetUp]
    public void Setup()
    {
        bossObject = new GameObject();
        spriteRenderer = bossObject.AddComponent<SpriteRenderer>();

        data = ScriptableObject.CreateInstance<BossData>();
        data.rageThreshold = 0.5f;
        data.speed = 2f;
        data.rageSpeedMultiplier = 2f;
        data.attackDamage = 10;
        data.rageDamageMultiplier = 1.5f;
        data.rageColor = Color.red;

        context = new BossContext
        {
            Data = data,
            Health = new BossHealth(100),
            SpriteRenderer = spriteRenderer,
            Rage = new BossRage()
        };
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(bossObject);
        Object.DestroyImmediate(data);
    }

    [Test]
    public void UpdateRageDoesNotTriggerAboveThreshold()
    {
        context.Health.TakeDamage(20);

        context.Rage.UpdateRage(context);

        Assert.IsFalse(context.Rage.IsRaging);
        Assert.AreNotEqual(Color.red, spriteRenderer.color);
    }

    [Test]
    public void UpdateRageTriggersWhenHealthDropsBelowThreshold()
    {
        context.Health.TakeDamage(60);

        context.Rage.UpdateRage(context);

        Assert.IsTrue(context.Rage.IsRaging);
        Assert.AreEqual(4f, context.CurrentSpeed);
        Assert.AreEqual(15, context.CurrentDamage);
        Assert.AreEqual(Color.red, spriteRenderer.color);
    }
}
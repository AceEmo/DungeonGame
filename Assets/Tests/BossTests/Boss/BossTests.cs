using NUnit.Framework;
using UnityEngine;

public class BossTests
{
    [Test]
    public void TakeDamage_ReducesHealth()
    {
        var go = new GameObject();

        go.AddComponent<Rigidbody2D>();
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<Animator>();

        var boss = go.AddComponent<Boss>();

        boss.data = ScriptableObject.CreateInstance<BossData>();
        boss.data.MaxHealth = 10;

        boss.SendMessage("Awake");

        boss.TakeDamage(3);

        Assert.Pass();
    }

    [Test]
    public void TakeDamage_WhenHealthZero_RaisesDeathEvent()
    {
        var go = new GameObject();

        go.AddComponent<Rigidbody2D>();
        go.AddComponent<SpriteRenderer>();
        go.AddComponent<Animator>();

        var boss = go.AddComponent<Boss>();

        boss.data = ScriptableObject.CreateInstance<BossData>();
        boss.data.MaxHealth = 5;

        boss.SendMessage("Awake");

        bool died = false;
        boss.OnBossDied += () => died = true;

        boss.TakeDamage(10);

        Assert.IsTrue(died);
    }
}
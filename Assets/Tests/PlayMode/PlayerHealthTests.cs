using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHealthTests
{
    private GameObject _playerObject;
    private PlayerHealth _health;
    private PlayerStats _stats;

    [SetUp]
    public void SetUp()
    {
        _playerObject = new GameObject("Player");
        _playerObject.AddComponent<Rigidbody2D>();
        _playerObject.AddComponent<SpriteRenderer>();
        _playerObject.AddComponent<BoxCollider2D>();
        _playerObject.AddComponent<AudioSource>();

        _stats = ScriptableObject.CreateInstance<PlayerStats>();
        _stats.startHealth = 10f;
        _stats.maxHealth = 10f;

        _health = _playerObject.AddComponent<PlayerHealth>();
        typeof(PlayerHealth).GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_health, _stats);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_playerObject);
        Object.Destroy(_stats);
    }

    [UnityTest]
    public IEnumerator Start_ShouldInitializeCurrentHealthToStartHealth()
    {
        _health.SendMessage("Awake");
        _health.SendMessage("Start");
        yield return null;

        Assert.AreEqual(10f, _health.CurrentHealth);
    }

    [UnityTest]
    public IEnumerator TakeDamage_ShouldReduceHealthAndTriggerInvincibility()
    {
        _health.SendMessage("Awake");
        _health.SendMessage("Start");
        yield return null;

        _health.TakeDamage(3f, Vector2.zero);

        Assert.AreEqual(7f, _health.CurrentHealth);
    }

    [UnityTest]
    public IEnumerator Heal_ShouldIncreaseHealthButNotExceedMaxHealth()
    {
        _health.SendMessage("Awake");
        _health.SendMessage("Start");
        yield return null;
        _health.TakeDamage(5f, Vector2.zero);

        _health.Heal(2f);

        Assert.AreEqual(7f, _health.CurrentHealth);

        _health.Heal(10f);
        Assert.AreEqual(10f, _health.CurrentHealth);
    }

    [UnityTest]
    public IEnumerator TakeDamage_WhenFatal_ShouldSetIsDeadToTrue()
    {
        _health.SendMessage("Awake");
        _health.SendMessage("Start");
        yield return null;

        _health.TakeDamage(20f, Vector2.zero);

        Assert.IsTrue(_health.IsDead);
    }
}
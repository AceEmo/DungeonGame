using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHealthTests
{
    private GameObject playerObject;
    private PlayerHealth playerHealth;
    private PlayerStats playerStats;
    private Rigidbody2D playerRigidbody;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;

        playerObject = new GameObject("PlayerTest");
        playerObject.SetActive(false);

        playerRigidbody = playerObject.AddComponent<Rigidbody2D>();
        playerRigidbody.bodyType = RigidbodyType2D.Dynamic;
        
        playerObject.AddComponent<SpriteRenderer>();
        playerObject.AddComponent<BoxCollider2D>();
        playerObject.AddComponent<PlayerMovement>();

        playerHealth = playerObject.AddComponent<PlayerHealth>();

        playerStats = ScriptableObject.CreateInstance<PlayerStats>();
        playerStats.startHealth = 6f;
        playerStats.maxHealth = 12f;

        FieldInfo statsField = typeof(PlayerHealth).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(playerHealth, playerStats);

        playerObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(playerStats);
    }

    [UnityTest]
    public IEnumerator StartSetsInitialHealthCorrectly()
    {
        yield return null;

        Assert.AreEqual(6f, playerHealth.CurrentHealth);
        Assert.AreEqual(12f, playerHealth.MaxHealth);
    }

    [UnityTest]
    public IEnumerator TakeDamageReducesHealth()
    {
        yield return null;

        playerHealth.TakeDamage(2f, Vector2.zero);

        Assert.AreEqual(4f, playerHealth.CurrentHealth);
    }

    [UnityTest]
    public IEnumerator TakeDamageCannotReduceHealthBelowZero()
    {
        yield return null;

        playerHealth.TakeDamage(100f, Vector2.zero);

        Assert.AreEqual(0f, playerHealth.CurrentHealth);
    }

    [UnityTest]
    public IEnumerator TakeDamageAppliesKnockback()
    {
        yield return null;

        Vector2 damageSource = new Vector2(5f, 0f);
        playerHealth.TakeDamage(1f, damageSource);

        Assert.IsTrue(playerRigidbody.linearVelocity.x < 0);
    }

    [UnityTest]
    public IEnumerator HealIncreasesHealth()
    {
        yield return null;

        playerHealth.TakeDamage(4f, Vector2.zero);
        playerHealth.Heal(3f);

        Assert.AreEqual(5f, playerHealth.CurrentHealth);
    }

    [UnityTest]
    public IEnumerator HealCannotExceedMaxHealth()
    {
        yield return null;

        playerHealth.Heal(50f);

        Assert.AreEqual(12f, playerHealth.CurrentHealth);
    }

    [UnityTest]
    public IEnumerator ResetHealthRestoresStatsValue()
    {
        yield return null;

        playerHealth.TakeDamage(5f, Vector2.zero);
        
        MethodInfo resetMethod = typeof(PlayerHealth).GetMethod("InitializeHealth", BindingFlags.NonPublic | BindingFlags.Instance);
        resetMethod.Invoke(playerHealth, null);

        Assert.AreEqual(6f, playerHealth.CurrentHealth);
    }
}
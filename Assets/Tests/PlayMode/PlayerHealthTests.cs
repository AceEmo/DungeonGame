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
        playerObject.AddComponent<SpriteRenderer>();
        playerObject.AddComponent<BoxCollider2D>();
        
        PlayerMovement playerMovement = playerObject.AddComponent<PlayerMovement>();
        playerMovement.enabled = false;

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

    [Test]
    public void AwakeSetsInitialHealthCorrectly()
    {
        Assert.AreEqual(6f, playerHealth.CurrentHealth);
        Assert.AreEqual(12f, playerHealth.MaxHealth);
    }

    [Test]
    public void TakeDamageReducesHealth()
    {
        playerHealth.TakeDamage(2f, Vector2.zero);

        Assert.AreEqual(4f, playerHealth.CurrentHealth);
    }

    [Test]
    public void TakeDamageCannotReduceHealthBelowZero()
    {
        playerHealth.TakeDamage(100f, Vector2.zero);

        Assert.AreEqual(0f, playerHealth.CurrentHealth);
    }

    [Test]
    public void TakeDamageAppliesKnockback()
    {
        Vector2 damageSource = new Vector2(5f, 0f);

        playerHealth.TakeDamage(1f, damageSource);

        Assert.IsTrue(playerRigidbody.linearVelocity.x < 0);
    }

    [Test]
    public void HealIncreasesHealth()
    {
        playerHealth.TakeDamage(4f, Vector2.zero);

        playerHealth.Heal(3f);

        Assert.AreEqual(5f, playerHealth.CurrentHealth);
    }

    [Test]
    public void HealCannotExceedMaxHealth()
    {
        playerHealth.Heal(50f);

        Assert.AreEqual(12f, playerHealth.CurrentHealth);
    }

    [Test]
    public void AddScrapIncreasesTotalScrapCount()
    {
        playerHealth.AddScrap(5);
        playerHealth.AddScrap(10);

        Assert.AreEqual(15, playerHealth.Scrap);
    }

    [Test]
    public void ResetScrapClearsTotalScrapCount()
    {
        playerHealth.AddScrap(10);

        playerHealth.ResetScrap();

        Assert.AreEqual(0, playerHealth.Scrap);
    }
}
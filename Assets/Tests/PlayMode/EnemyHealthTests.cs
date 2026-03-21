using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyHealthTests
{
    private GameObject enemyObject;
    private EnemyHealth enemyHealth;
    private EnemyData enemyData;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [SetUp]
    public void Setup()
    {
        enemyObject = new GameObject("TestEnemy");
        enemyObject.SetActive(false);

        Enemy enemyComponent = enemyObject.AddComponent<Enemy>();
        enemyComponent.enabled = false;

        spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
        animator = enemyObject.AddComponent<Animator>();
        enemyObject.AddComponent<BoxCollider2D>();
        enemyObject.AddComponent<Rigidbody2D>();
        
        enemyData = ScriptableObject.CreateInstance<EnemyData>();
        enemyData.MaxHealth = 5;
        enemyData.hitColor = Color.red;
        enemyComponent.data = enemyData;

        enemyHealth = enemyObject.AddComponent<EnemyHealth>();
        
        enemyObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(enemyObject);
        Object.DestroyImmediate(enemyData);
    }

    [Test]
    public void TakeDamageReducesHealth()
    {
        enemyHealth.TakeDamage(2);

        FieldInfo healthField = typeof(EnemyHealth).GetField("CurrentHealth", BindingFlags.NonPublic | BindingFlags.Instance);
        int currentHealth = (int)healthField.GetValue(enemyHealth);

        Assert.AreEqual(3, currentHealth);
    }

    [UnityTest]
    public IEnumerator TakeDamageTriggersHitFlash()
    {
        Color originalColor = spriteRenderer.color;

        enemyHealth.TakeDamage(1);

        yield return null;

        Assert.AreEqual(Color.red, spriteRenderer.color);

        yield return new WaitForSeconds(0.15f);

        Assert.AreEqual(originalColor, spriteRenderer.color);
    }

    [UnityTest]
    public IEnumerator DieDisablesComponentsAndDestroysObject()
    {
        enemyHealth.TakeDamage(5);

        Assert.IsTrue(enemyHealth.IsEnemyDead());

        Collider2D collider = enemyObject.GetComponent<Collider2D>();
        Assert.IsFalse(collider.enabled);

        Rigidbody2D rb = enemyObject.GetComponent<Rigidbody2D>();
        Assert.IsFalse(rb.simulated);

        yield return new WaitForSeconds(1.6f);

        Assert.IsTrue(enemyObject == null);
    }
}
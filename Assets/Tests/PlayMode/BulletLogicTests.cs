using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BulletLogicTests
{
    private GameObject bulletObject;
    private BulletLogic bulletLogic;

    [SetUp]
    public void Setup()
    {
        bulletObject = new GameObject("Bullet");
        bulletLogic = bulletObject.AddComponent<BulletLogic>();
    }

    [TearDown]
    public void Teardown()
    {
        if (bulletObject != null)
        {
            Object.DestroyImmediate(bulletObject);
        }
    }

    [Test]
    public void SetDamageAssignsCorrectValue()
    {
        bulletLogic.SetDamage(5);

        FieldInfo damageField = typeof(BulletLogic).GetField("damage", BindingFlags.NonPublic | BindingFlags.Instance);
        int damageValue = (int)damageField.GetValue(bulletLogic);

        Assert.AreEqual(5, damageValue);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2DWithEnemyDealsDamageAndDestroysBullet()
    {
        bulletLogic.SetDamage(3);

        GameObject enemyObject = new GameObject("Enemy");
        enemyObject.tag = "Enemy";
        BoxCollider2D enemyCollider = enemyObject.AddComponent<BoxCollider2D>();
        MockDamageable mockDamageable = enemyObject.AddComponent<MockDamageable>();

        MethodInfo onTriggerEnterMethod = typeof(BulletLogic).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        onTriggerEnterMethod.Invoke(bulletLogic, new object[] { enemyCollider });

        yield return null;

        Assert.AreEqual(3, mockDamageable.damageTaken);
        Assert.IsTrue(bulletObject == null);

        Object.DestroyImmediate(enemyObject);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2DWithPlayerIgnoresCollision()
    {
        GameObject playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        BoxCollider2D playerCollider = playerObject.AddComponent<BoxCollider2D>();

        MethodInfo onTriggerEnterMethod = typeof(BulletLogic).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        onTriggerEnterMethod.Invoke(bulletLogic, new object[] { playerCollider });

        yield return null;

        Assert.IsFalse(bulletObject == null);

        Object.DestroyImmediate(playerObject);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2DWithWallDestroysBullet()
    {
        GameObject wallObject = new GameObject("Wall");
        BoxCollider2D wallCollider = wallObject.AddComponent<BoxCollider2D>();

        MethodInfo onTriggerEnterMethod = typeof(BulletLogic).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        onTriggerEnterMethod.Invoke(bulletLogic, new object[] { wallCollider });

        yield return null;

        Assert.IsTrue(bulletObject == null);

        Object.DestroyImmediate(wallObject);
    }

    [UnityTest]
    public IEnumerator BulletDestroysItselfAfterLifetime()
    {
        FieldInfo lifetimeField = typeof(BulletLogic).GetField("lifetime", BindingFlags.NonPublic | BindingFlags.Instance);
        lifetimeField.SetValue(bulletLogic, 0.1f);

        MethodInfo startMethod = typeof(BulletLogic).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(bulletLogic, null);

        yield return new WaitForSeconds(0.15f);

        Assert.IsTrue(bulletObject == null);
    }

    private class MockDamageable : MonoBehaviour, IDamageable
    {
        public int damageTaken;

        public void TakeDamage(int amount)
        {
            damageTaken += amount;
        }
    }
}
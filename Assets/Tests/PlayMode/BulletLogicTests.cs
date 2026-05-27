using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BulletLogicTests
{
    private GameObject _bulletObject;
    private BulletLogic _bulletLogic;

    [SetUp]
    public void SetUp()
    {
        _bulletObject = new GameObject("Bullet");
        _bulletObject.AddComponent<BoxCollider2D>();
        _bulletLogic = _bulletObject.AddComponent<BulletLogic>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_bulletObject != null) Object.Destroy(_bulletObject);
    }

    [UnityTest]
    public IEnumerator Start_ShouldDestroyBulletAfterLifetime()
    {
        yield return new WaitForSeconds(3.2f);

        Assert.IsTrue(_bulletObject == null);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WhenHittingEnemy_ShouldApplyDamageAndDestroyBullet()
    {
        _bulletLogic.SetDamage(10);

        GameObject enemyObject = new GameObject("Enemy");
        enemyObject.tag = "Enemy";
        enemyObject.AddComponent<BoxCollider2D>();
        MockDamageable mockDamageable = enemyObject.AddComponent<MockDamageable>();

        var enemyCollider = enemyObject.GetComponent<BoxCollider2D>();
        _bulletLogic.SendMessage("OnTriggerEnter2D", enemyCollider);

        yield return null;

        Assert.IsTrue(mockDamageable.DamageTaken == 10);
        Assert.IsTrue(_bulletObject == null);

        Object.Destroy(enemyObject);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WhenHittingNonPlayerObstacle_ShouldDestroyBullet()
    {
        GameObject wallObject = new GameObject("Wall");
        wallObject.tag = "Untagged";
        var wallCollider = wallObject.AddComponent<BoxCollider2D>();

        _bulletLogic.SendMessage("OnTriggerEnter2D", wallCollider);

        yield return null;

        Assert.IsTrue(_bulletObject == null);

        Object.Destroy(wallObject);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WhenHittingPlayer_ShouldNotDestroyBullet()
    {
        GameObject playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        var playerCollider = playerObject.AddComponent<BoxCollider2D>();

        _bulletLogic.SendMessage("OnTriggerEnter2D", playerCollider);

        yield return null;

        Assert.IsNotNull(_bulletObject);

        Object.Destroy(playerObject);
    }

    private class MockDamageable : MonoBehaviour, IDamageable
    {
        public int DamageTaken { get; private set; }

        public void TakeDamage(int amount)
        {
            DamageTaken = amount;
        }
    }
}
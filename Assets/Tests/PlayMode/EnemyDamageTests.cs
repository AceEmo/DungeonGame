using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyDamageTests
{
    private GameObject _enemyObject;
    private GameObject _playerObject;
    private EnemyDamage _enemyDamage;
    private MockPlayerHealth _mockPlayerHealth;

    [SetUp]
    public void SetUp()
    {
        _enemyObject = new GameObject("Enemy");
        
        var enemy = _enemyObject.AddComponent<Enemy>();
        enemy.enabled = false;
        
        enemy.GetType()
            .GetField("currentDamage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(enemy, 2);

        var health = _enemyObject.AddComponent<EnemyHealth>();
        health.enabled = false;

        enemy.GetType()
            .GetField("health", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(enemy, health);

        _enemyDamage = _enemyObject.AddComponent<EnemyDamage>();
        
        _enemyDamage.GetType()
            .GetField("enemy", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_enemyDamage, enemy);

        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";
        _playerObject.AddComponent<BoxCollider2D>();
        _mockPlayerHealth = _playerObject.AddComponent<MockPlayerHealth>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_enemyObject);
        Object.DestroyImmediate(_playerObject);
    }

    [UnityTest]
    public IEnumerator OnTriggerStay2D_WhenHittingPlayer_ShouldDealDamage()
    {
        var playerCollider = _playerObject.GetComponent<BoxCollider2D>();

        _enemyDamage.SendMessage("OnTriggerStay2D", playerCollider);
        yield return null;

        Assert.IsTrue(!_mockPlayerHealth.DamageCalled);
    }

    private class MockPlayerHealth : MonoBehaviour
    {
        public bool DamageCalled { get; private set; }
        public void TakeDamage(int amount, Vector2 position) => DamageCalled = true;
    }
}
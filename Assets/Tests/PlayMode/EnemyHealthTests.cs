using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyHealthTests
{
    private GameObject _enemyObject;
    private EnemyHealth _enemyHealth;

    [SetUp]
    public void SetUp()
    {
        _enemyObject = new GameObject("Enemy");
        
        var enemy = _enemyObject.AddComponent<Enemy>();
        enemy.enabled = false;
        
        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.MaxHealth = 10;
        
        enemy.GetType()
            .GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(enemy, data);
            
        enemy.GetType()
            .GetField("currentMaxHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(enemy, 10);

        _enemyObject.AddComponent<MockMovement>();
        _enemyObject.AddComponent<MockBehaviour>();
        _enemyObject.AddComponent<SpriteRenderer>();

        _enemyHealth = _enemyObject.AddComponent<EnemyHealth>();
        _enemyHealth.enabled = false;
    }

    [TearDown]
    public void TearDown()
    {
        if (_enemyObject != null) 
        {
            Object.DestroyImmediate(_enemyObject);
        }
    }

    [UnityTest]
    public IEnumerator TakeDamage_ShouldReduceHealthAndTriggerDeath()
    {
        _enemyHealth.SendMessage("Awake");
        _enemyHealth.SendMessage("Start");
        yield return null;

        _enemyHealth.TakeDamage(10);
        yield return null;

        Assert.IsTrue(_enemyHealth.IsEnemyDead());
    }

    private class MockMovement : MonoBehaviour, IEnemyMovement
    {
        public void Move(Vector2 direction) {}
    }

    private class MockBehaviour : MonoBehaviour, IEnemyBehaviour
    {
        public Vector2 GetDirection() => Vector2.zero;
    }
}
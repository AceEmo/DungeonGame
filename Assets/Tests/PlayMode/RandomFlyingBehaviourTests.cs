using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RandomFlyingBehaviourTests
{
    private GameObject _enemyObject;
    private Enemy _enemy;
    private RandomFlyingBehaviour _flyingBehaviour;

    [SetUp]
    public void SetUp()
    {
        _enemyObject = new GameObject();
        _enemy = _enemyObject.AddComponent<Enemy>();
        _enemy.GetType().GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_enemy, ScriptableObject.CreateInstance<EnemyData>());
        _enemy.Data.changeDirectionInterval = 0.1f;

        _flyingBehaviour = _enemyObject.AddComponent<RandomFlyingBehaviour>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_enemyObject);
    }

    [UnityTest]
    public IEnumerator GetDirection_ShouldReturnValidNormalizedVector()
    {
        _flyingBehaviour.SendMessage("Start");
        yield return null;

        Vector2 direction = _flyingBehaviour.GetDirection();

        Assert.AreEqual(1f, direction.magnitude, 0.01f);
    }

    [UnityTest]
    public IEnumerator GetDirection_AfterInterval_ShouldChangeDirection()
    {
        _flyingBehaviour.SendMessage("Start");
        yield return null;

        Vector2 firstDirection = _flyingBehaviour.GetDirection();
        yield return new WaitForSeconds(0.15f);
        Vector2 secondDirection = _flyingBehaviour.GetDirection();

        Assert.AreNotEqual(firstDirection, secondDirection);
    }
}
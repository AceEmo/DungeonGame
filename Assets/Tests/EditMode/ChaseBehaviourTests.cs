using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ChaseBehaviourTests
{
    private GameObject _enemyObject;
    private GameObject _playerObject;
    private ChaseBehaviour _chaseBehaviour;

    [SetUp]
    public void SetUp()
    {
        _enemyObject = new GameObject("Enemy");
        _playerObject = new GameObject("Player");
        
        _chaseBehaviour = _enemyObject.AddComponent<ChaseBehaviour>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_enemyObject);
        Object.DestroyImmediate(_playerObject);
    }

    [Test]
    public void GetDirection_WhenPlayerExists_ShouldReturnNormalizedDirectionTowardsPlayer()
    {
        _enemyObject.transform.position = Vector3.zero;
        _playerObject.transform.position = new Vector3(3f, 4f, 0f);

        typeof(ChaseBehaviour)
            .GetField("player", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_chaseBehaviour, _playerObject.transform);

        Vector2 direction = _chaseBehaviour.GetDirection();

        Assert.AreEqual(new Vector2(0.6f, 0.8f), direction);
    }
}
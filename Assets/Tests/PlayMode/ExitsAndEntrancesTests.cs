using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyMovementTests
{
    private GameObject _enemyObject;
    private Rigidbody2D _rigidbody;
    private FlyingMovement _flyingMovement;
    private GroundMovement _groundMovement;

    [SetUp]
    public void SetUp()
    {
        _enemyObject = new GameObject("EnemyMovementTestObject");
        
        _rigidbody = _enemyObject.AddComponent<Rigidbody2D>();
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = 0f;

        var data = ScriptableObject.CreateInstance<EnemyData>();
        data.speed = 5f;

        _flyingMovement = _enemyObject.AddComponent<FlyingMovement>();
        _flyingMovement.enabled = false;
        
        typeof(FlyingMovement)
            .GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_flyingMovement, data);
            
        typeof(FlyingMovement)
            .GetField("rb", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_flyingMovement, _rigidbody);

        _groundMovement = _enemyObject.AddComponent<GroundMovement>();
        _groundMovement.enabled = false;
        
        typeof(GroundMovement)
            .GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_groundMovement, data);
            
        typeof(GroundMovement)
            .GetField("rb", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(_groundMovement, _rigidbody);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_enemyObject);
    }

    [UnityTest]
    public IEnumerator FlyingMovement_ShouldApplyVelocityDirectly()
    {
        _flyingMovement.Move(Vector2.right);
        yield return new WaitForFixedUpdate();

        Assert.AreEqual(new Vector2(5f, 0f), _rigidbody.linearVelocity);
    }

    [UnityTest]
    public IEnumerator GroundMovement_ShouldApplyVelocityWithSmoothing()
    {
        _groundMovement.Move(Vector2.up);
        yield return new WaitForFixedUpdate();

        Assert.Greater(_rigidbody.linearVelocity.y, 0f);
    }
}
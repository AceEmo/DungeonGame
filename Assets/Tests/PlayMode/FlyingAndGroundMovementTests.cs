using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FlyingAndGroundMovementTests
{
    private GameObject _flyingEnemy;
    private GameObject _groundEnemy;
    private Rigidbody2D _flyingRb;
    private Rigidbody2D _groundRb;
    private FlyingMovement _flyingMovement;
    private GroundMovement _groundMovement;

    [SetUp]
    public void SetUp()
    {
        _flyingEnemy = new GameObject();
        var enemy1 = _flyingEnemy.AddComponent<Enemy>();
        enemy1.GetType().GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(enemy1, ScriptableObject.CreateInstance<EnemyData>());
        _flyingRb = _flyingEnemy.AddComponent<Rigidbody2D>();
        _flyingRb.bodyType = RigidbodyType2D.Dynamic;
        _flyingMovement = _flyingEnemy.AddComponent<FlyingMovement>();
        _flyingEnemy.AddComponent<MockBehaviour>();

        _groundEnemy = new GameObject();
        var enemy2 = _groundEnemy.AddComponent<Enemy>();
        enemy2.GetType().GetField("data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(enemy2, ScriptableObject.CreateInstance<EnemyData>());
        _groundRb = _groundEnemy.AddComponent<Rigidbody2D>();
        _groundRb.bodyType = RigidbodyType2D.Dynamic;
        _groundMovement = _groundEnemy.AddComponent<GroundMovement>();
        _groundEnemy.AddComponent<MockBehaviour>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_flyingEnemy);
        Object.Destroy(_groundEnemy);
    }

    [UnityTest]
    public IEnumerator FlyingMovement_ShouldApplyVelocityDirectly()
    {
        _flyingEnemy.SendMessage("Awake");
        yield return null;

        _flyingMovement.Move(Vector2.right);
        yield return new WaitForFixedUpdate();

        Assert.AreEqual(Vector2.right * 3f, _flyingRb.linearVelocity);
    }

    [UnityTest]
    public IEnumerator GroundMovement_ShouldApplyVelocityWithSmoothing()
    {
        _groundEnemy.SendMessage("Awake");
        yield return null;

        _groundMovement.Move(Vector2.up);
        yield return new WaitForFixedUpdate();

        Assert.Greater(_groundRb.linearVelocity.y, 0f);
    }

    private class MockBehaviour : MonoBehaviour, IEnemyBehaviour
    {
        public Vector2 GetDirection() => Vector2.zero;
    }
}
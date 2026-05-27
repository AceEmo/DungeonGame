using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BossMovementTests
{
    private GameObject _bossGameObject;
    private Rigidbody2D _rigidbody;
    private BossMovement _bossMovement;

    [SetUp]
    public void SetUp()
    {
        _bossGameObject = new GameObject();
        _rigidbody = _bossGameObject.AddComponent<Rigidbody2D>();
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = 0f;
        
        _bossMovement = new BossMovement(_rigidbody);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_bossGameObject);
    }

    [UnityTest]
    public IEnumerator Move_ShouldApplyVelocityToRigidbody()
    {
        _bossMovement.Move(Vector2.right, 5f);

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(new Vector2(5f, 0f), _rigidbody.linearVelocity);
    }

    [UnityTest]
    public IEnumerator Stop_ShouldSetVelocityToZero()
    {
        _bossMovement.Move(Vector2.up, 10f);
        yield return new WaitForFixedUpdate();

        _bossMovement.Stop();
        yield return new WaitForFixedUpdate();

        Assert.AreEqual(Vector2.zero, _rigidbody.linearVelocity);
    }
}
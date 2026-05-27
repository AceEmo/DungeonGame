using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BossComponentsTests
{
    private GameObject _bossHolder;
    private Boss _boss;

    [SetUp]
    public void SetUp()
    {
        _bossHolder = new GameObject();
        _bossHolder.AddComponent<SpriteRenderer>();
        _bossHolder.AddComponent<Animator>();
        _bossHolder.AddComponent<Rigidbody2D>();
        
        _boss = _bossHolder.AddComponent<Boss>();
        _boss.enabled = false;
        _boss.data = ScriptableObject.CreateInstance<BossData>();
        _boss.data.hitColor = Color.red;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_bossHolder);
    }

    [UnityTest]
    public IEnumerator TakeDamage_WhenDead_ShouldInvokeOnBossDied()
    {
        bool eventInvoked = false;
        _boss.OnBossDied += () => eventInvoked = true;

        var context = typeof(Boss)
            .GetField("context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(_boss) as BossContext;

        if (context != null)
        {
            context.Health = new BossHealth(10);
        }

        _boss.TakeDamage(100);
        yield return null;

        Assert.IsTrue(eventInvoked);
    }
}
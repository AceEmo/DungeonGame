using NUnit.Framework;
using UnityEngine;

public class DeathStateTests
{
    [Test]
    public void EnterState_DisablesRigidbody()
    {
        var go = new GameObject();
        var rb = go.AddComponent<Rigidbody2D>();
        var animator = go.AddComponent<Animator>();
        var boss = go.AddComponent<Boss>();

        var context = new BossContext
        {
            BossTransform = go.transform,
            Animator = animator
        };

        var state = new DeathState();

        state.EnterState(context);

        Assert.IsFalse(rb.simulated);
    }
}
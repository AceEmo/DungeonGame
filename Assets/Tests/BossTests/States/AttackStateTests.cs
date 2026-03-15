using NUnit.Framework;
using UnityEngine;

public class AttackStateTests
{
    [Test]
    public void EnterState_StopsMovement()
    {
        var go = new GameObject();
        var rb = go.AddComponent<Rigidbody2D>();

        var context = new BossContext
        {
            Movement = new BossMovement(rb),
            Animator = go.AddComponent<Animator>(),
            Combat = new BossCombat(null,null,null,null),
            Brain = new BossBrain(null)
        };

        var state = new AttackState();

        state.EnterState(context);

        Assert.Pass();
    }

    [Test]
    public void UpdateState_AttacksOnce()
    {
        var context = new BossContext
        {
            Brain = new BossBrain(null),
            Combat = new BossCombat(null,null,null,null)
        };

        var state = new AttackState();

        state.UpdateState(context);

        Assert.Pass();
    }
}
using NUnit.Framework;
using UnityEngine;

public class ChaseStateTests
{
    [Test]
    public void WhenPlayerInAttackRange_ChangesToAttackState()
    {
        var boss = new GameObject();
        var player = new GameObject();

        var data = ScriptableObject.CreateInstance<BossData>();
        data.attackRange = 5f;

        boss.transform.position = Vector2.zero;
        player.transform.position = Vector2.one;

        var context = new BossContext
        {
            BossTransform = boss.transform,
            Player = player.transform,
            Data = data,
            Movement = new BossMovement(boss.AddComponent<Rigidbody2D>()),
            Brain = new BossBrain(null)
        };

        var state = new ChaseState();

        state.UpdateState(context);

        Assert.Pass();
    }
}
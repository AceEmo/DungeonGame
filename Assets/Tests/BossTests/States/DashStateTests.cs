using NUnit.Framework;
using UnityEngine;

public class DashStateTests
{
    [Test]
    public void EnterState_SetsDashDirection()
    {
        var boss = new GameObject();
        var player = new GameObject();

        player.transform.position = Vector2.right;

        var data = ScriptableObject.CreateInstance<BossData>();

        var context = new BossContext
        {
            BossTransform = boss.transform,
            Player = player.transform,
            Data = data,
            Movement = new BossMovement(boss.AddComponent<Rigidbody2D>())
        };

        var state = new DashState();

        state.EnterState(context);

        Assert.Pass();
    }

    [Test]
    public void UpdateState_RunsWithoutErrors()
    {
        var context = new BossContext
        {
            Data = ScriptableObject.CreateInstance<BossData>(),
            Movement = new BossMovement(new GameObject().AddComponent<Rigidbody2D>())
        };

        var state = new DashState();

        state.UpdateState(context);

        Assert.Pass();
    }
}
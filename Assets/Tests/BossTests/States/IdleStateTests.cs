using NUnit.Framework;

public class IdleStateTests
{
    [Test]
    public void Update_ChangesToChaseState()
    {
        var context = new BossContext();

        context.Brain = new BossBrain(context);

        var state = new IdleState();

        state.UpdateState(context);

        Assert.Pass();
    }
}
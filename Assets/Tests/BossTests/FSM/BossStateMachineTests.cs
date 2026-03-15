using NUnit.Framework;

public class BossStateMachineTests
{
    private class TestState : IBossState
    {
        public bool entered;
        public bool exited;

        public void EnterState(BossContext context)
        {
            entered = true;
        }

        public void UpdateState(BossContext context) { }

        public void ExitState(BossContext context)
        {
            exited = true;
        }
    }

    [Test]
    public void SetState_CallsEnterState()
    {
        var fsm = new BossStateMachine();
        var ctx = new BossContext();

        var state = new TestState();

        fsm.SetState(state, ctx);

        Assert.IsTrue(state.entered);
    }

    [Test]
    public void ChangingState_CallsExitState()
    {
        var fsm = new BossStateMachine();
        var ctx = new BossContext();

        var state1 = new TestState();
        var state2 = new TestState();

        fsm.SetState(state1, ctx);
        fsm.SetState(state2, ctx);

        Assert.IsTrue(state1.exited);
    }
}
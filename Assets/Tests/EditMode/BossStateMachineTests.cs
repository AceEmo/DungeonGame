using NUnit.Framework;

[TestFixture]
public class BossStateMachineTests
{
    private BossStateMachine _stateMachine;
    private BossContext _context;
    private MockBossState _initialState;
    private MockBossState _nextState;

    [SetUp]
    public void SetUp()
    {
        _stateMachine = new BossStateMachine();
        _context = new BossContext();
        _initialState = new MockBossState();
        _nextState = new MockBossState();
    }

    [Test]
    public void SetState_ShouldCallEnterOnNewState()
    {
        _stateMachine.SetState(_initialState, _context);

        Assert.IsTrue(_initialState.EnterCalled);
    }

    [Test]
    public void SetState_WhenChangingState_ShouldCallExitOnOldStateAndEnterOnNewState()
    {
        _stateMachine.SetState(_initialState, _context);
        
        _stateMachine.SetState(_nextState, _context);

        Assert.IsTrue(_initialState.ExitCalled);
        Assert.IsTrue(_nextState.EnterCalled);
    }

    [Test]
    public void Update_ShouldCallUpdateOnCurrentState()
    {
        _stateMachine.SetState(_initialState, _context);

        _stateMachine.Update(_context);

        Assert.IsTrue(_initialState.UpdateCalled);
    }

    private class MockBossState : IBossState
    {
        public bool EnterCalled { get; private set; }
        public bool UpdateCalled { get; private set; }
        public bool ExitCalled { get; private set; }

        public void EnterState(BossContext context) => EnterCalled = true;
        public void UpdateState(BossContext context) => UpdateCalled = true;
        public void ExitState(BossContext context) => ExitCalled = true;
    }
}
public class BossStateMachine
{
    private IBossState currentState;

    public void SetState(IBossState state, BossContext context)
    {
        currentState?.ExitState(context);

        currentState = state;

        currentState.EnterState(context);
    }

    public void Update(BossContext context)
    {
        currentState?.UpdateState(context);
    }
}

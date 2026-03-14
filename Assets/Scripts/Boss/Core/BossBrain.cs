public class BossBrain
{
    private BossStateMachine fsm;

    private BossContext context;

    public BossBrain(BossContext ctx)
    {
        context = ctx;

        fsm = new BossStateMachine();
    }

    public void Start()
    {
        ChangeState(new IdleState());
    }

    public void Update()
    {
        context.Rage.UpdateRage(context);

        fsm.Update(context);
    }

    public void ChangeState(IBossState state)
    {
        fsm.SetState(state, context);
    }
}
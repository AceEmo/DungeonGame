using UnityEngine;

public class IdleState : IBossState
{
    public void EnterState(BossContext context)
    {
        context.Movement.Stop();
    }

    public void UpdateState(BossContext context)
    {
        if (context.IsDead)
            return;
        context.Brain.ChangeState(new ChaseState());
    }

    public void ExitState(BossContext context) { }
}

public class AttackState : IBossState
{
    private bool attacked;

    public void EnterState(BossContext context)
    {
        attacked = false;

        context.Movement.Stop();

        context.Animator.SetTrigger("Attack");
    }


    public void UpdateState(BossContext context)
    {
        if (context.IsDead)
            return;

        if (!attacked)
        {
            attacked = true;

            context.Combat.DealDamage(context);
        }

        context.Brain.ChangeState(new ChaseState());
    }

    public void ExitState(BossContext context) { }
}

using UnityEngine;

public class AttackState : IBossState
{
    private bool attacked = false;

    public void EnterState(Boss boss)
    {
        boss.StopMoving();
        boss.AnimatorComponent.SetTrigger("Attack");
        attacked = false;
    }

    public void UpdateState(Boss boss)
    {
        if (boss.IsDead) return;
        if (!attacked)
        {
            attacked = true;
            boss.DealDamageToPlayer();
        }

        if (attacked && !boss.AnimatorComponent.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            boss.SetState(new ChaseState());
        }
    }

    public void ExitState(Boss boss) { }
}
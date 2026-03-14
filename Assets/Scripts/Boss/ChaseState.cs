using UnityEngine;

public class ChaseState : IBossState
{
    public void EnterState(Boss boss) { }

    public void UpdateState(Boss boss)
    {
        if (boss.IsDead) return;

        float distance = Vector2.Distance(boss.transform.position, boss.Player.position);
        
        if (distance <= boss.Data.attackRange)
        {
            boss.SetState(new AttackState());
            return;
        }

        bool canDash = Time.time >= boss.LastDashTime + boss.Data.dashCooldown;
        bool closeEnough = distance <= boss.Data.dashTriggerDistance;
        bool chance = Random.value <= boss.Data.dashChance;

        if (canDash && closeEnough && chance)
        {
            boss.SetState(new DashState());
            return;
        }

        Vector2 dir = (boss.Player.position - boss.transform.position).normalized;
        boss.Move(dir); 
    }

    public void ExitState(Boss boss)
    {
        boss.StopMoving();
    }
}
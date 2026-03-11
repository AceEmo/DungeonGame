using UnityEngine;

public class ChaseState : IBossState
{
    public void EnterState(Boss boss) { }

    public void UpdateState(Boss boss)
    {
        if (boss.IsDead || boss.Player == null) return;

        Vector2 dir = (boss.Player.position - boss.transform.position).normalized;
        boss.Move(dir);

        float distance = Vector2.Distance(boss.transform.position, boss.Player.position);
        if (distance < boss.attackRange)
        {
            boss.SetState(new AttackState());
        }
    }

    public void ExitState(Boss boss)
    {
        boss.StopMoving();
    }
}
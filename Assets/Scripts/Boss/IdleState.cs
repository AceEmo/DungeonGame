using UnityEngine;

public class IdleState : IBossState
{
    public void EnterState(Boss boss)
    {
        boss.StopMoving();
    }

    public void UpdateState(Boss boss)
    {
        if (boss.IsDead || boss.Player == null) return;

        float distance = Vector2.Distance(boss.transform.position, boss.Player.position);
        if (distance < boss.attackRange * 3)
        {
            boss.SetState(new ChaseState());
        }
    }

    public void ExitState(Boss boss) { }
}
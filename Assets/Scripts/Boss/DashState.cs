using UnityEngine;

public class DashState : IBossState
{
    private float timer;
    private Vector2 dashDir;

    public void EnterState(Boss boss)
    {
        timer = boss.Data.dashDuration;

        if (boss.Player != null)
            dashDir = (boss.Player.position - boss.transform.position).normalized;
    }

    public void UpdateState(Boss boss)
    {
        if (boss.IsDead) return;

        boss.Move(dashDir * boss.Data.dashSpeed);

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            boss.SetState(new ChaseState());
        }
    }

    public void ExitState(Boss boss)
    {
        boss.StopMoving();
    }
}
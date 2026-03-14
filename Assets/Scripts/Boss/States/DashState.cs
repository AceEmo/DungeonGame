using UnityEngine;

public class DashState : IBossState
{
    private float dashTimer;
    private Vector2 dashDir;

    public void EnterState(BossContext context)
    {
        if (context.IsDead)
            return;
        dashTimer = context.Data.dashDuration;

        if (context.Player != null)
        {
            dashDir =
                (context.Player.position -
                context.BossTransform.position)
                .normalized;
        }
    }

    public void UpdateState(BossContext context)
    {
        context.Movement.Move(
            dashDir,
            context.Data.dashSpeed);

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            context.LastDashTime = Time.time;

            context.Brain.ChangeState(new ChaseState());
        }
    }

    public void ExitState(BossContext context)
    {
        context.Movement.Stop();
    }
}

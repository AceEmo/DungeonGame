using UnityEngine;

public class ChaseState : IBossState
{
    public void EnterState(BossContext context) { }

    public void UpdateState(BossContext context)
    {
        if (context.IsDead)
            return;

        Vector2 bossPos = context.BossTransform.position;
        Vector2 playerPos = context.Player.position;

        float distance = Vector2.Distance(bossPos, playerPos);

        if (distance <= context.Data.attackRange)
        {
            context.Brain.ChangeState(new AttackState());
            return;
        }

        bool canDash =
            Time.time >= context.LastDashTime +
            context.Data.dashCooldown;

        bool closeEnough =
            distance <= context.Data.dashTriggerDistance;

        if (canDash && closeEnough)
        {
            if (Random.value <= context.Data.dashChance)
            {
                context.Brain.ChangeState(new DashState());
                return;
            }
        }

        Vector2 dir = (playerPos - bossPos).normalized;

        context.LastMoveDirection = dir;

        context.Animator.SetFloat("MoveX", dir.x);
        context.Animator.SetFloat("MoveY", dir.y);

        context.Movement.Move(dir, context.Data.speed);
    }

    public void ExitState(BossContext context)
    {
        context.Movement.Stop();
    }
}

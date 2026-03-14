using UnityEngine;
using System.Collections;

public class DashState : IBossState
{
    private float dashTimer;
    private Vector2 dashDir;
    private bool dashStarted = false;

    public void EnterState(Boss boss)
    {
        dashStarted = false;
        dashTimer = boss.Data.dashDuration;

        boss.StartCoroutine(DashWindup(boss));
    }

    private IEnumerator DashWindup(Boss boss)
    {
        yield return new WaitForSeconds(boss.Data.dashWindup);

        if (boss.Player != null)
            dashDir = (boss.Player.position - boss.transform.position).normalized;

        dashStarted = true;
    }

    public void UpdateState(Boss boss)
    {
        if (boss.IsDead) return;

        if (!dashStarted)
        {
            boss.StopMoving();
            return;
        }

        boss.Move(dashDir * boss.Data.dashSpeed);

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            boss.StartDashCooldown();
            boss.SetState(new ChaseState());
        }
    }

    public void ExitState(Boss boss)
    {
        boss.StopMoving();
    }
}
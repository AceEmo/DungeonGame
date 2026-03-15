using UnityEngine;

public class DeathState : IBossState
{
    public void EnterState(BossContext context)
    {
        context.Movement.Stop();

        var rigidbody = context.BossTransform.GetComponent<Rigidbody2D>();
        if (rigidbody != null)
            rigidbody.simulated = false;

        if (context.Animator != null)
            context.Animator.SetTrigger("Die");

        var boss = context.BossTransform.GetComponent<Boss>();
        if (boss != null)
            boss.StartFadeAndDestroy();
    }

    public void UpdateState(BossContext context) {}
    public void ExitState(BossContext context) {}
}

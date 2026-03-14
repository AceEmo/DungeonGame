using UnityEngine;

public class DeathState : IBossState
{
    public void EnterState(Boss boss)
    {
        boss.StopMoving();
        boss.AnimatorComponent.SetTrigger("Die");
    }

    public void UpdateState(Boss boss) { }
    public void ExitState(Boss boss) { }
}
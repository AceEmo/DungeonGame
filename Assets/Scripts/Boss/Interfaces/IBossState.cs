public interface IBossState
{
    void EnterState(Boss boss);
    void UpdateState(Boss boss);
    void ExitState(Boss boss);
}
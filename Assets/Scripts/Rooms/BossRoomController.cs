using UnityEngine;

public class BossRoomController : MonoBehaviour
{
    public VentController vent;
    
    private Boss currentBoss;

    public void InitializeBoss(Boss spawnedBoss)
    {
        currentBoss = spawnedBoss;
        currentBoss.OnBossDied += HandleBossDeath;
    }

    private void OnDisable()
    {
        if (currentBoss != null)
        {
            currentBoss.OnBossDied -= HandleBossDeath;
        }
    }

    private void HandleBossDeath()
    {
        if (vent != null)
        {
            vent.OpenVent();
        }
    }
}
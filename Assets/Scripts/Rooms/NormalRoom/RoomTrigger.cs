using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private bool isTriggered = false;
    private Rooms room;
    private EnemyManager enemyManager;

    private void Awake()
    {
        room = GetComponentInParent<Rooms>();
        enemyManager = GetComponentInParent<EnemyManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;
        if (!other.CompareTag("Player")) return;

        isTriggered = true;

        if (room != null)
        {
            room.LockAllDoors();
        }

        if (enemyManager != null)
        {
            enemyManager.SpawnEnemiesOnEnter();
        }
    }
}
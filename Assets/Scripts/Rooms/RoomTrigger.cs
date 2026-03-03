using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private bool triggered = false;
    private Rooms room;
    private EnemyManager enemyManager;

    private void Awake()
    {
        room = GetComponentInParent<Rooms>();
        enemyManager = GetComponentInParent<EnemyManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (!room.IsStarter)
        {
            if (room.LeftDoor) room.LeftDoor.Lock();
            if (room.RightDoor) room.RightDoor.Lock();
            if (room.TopDoor) room.TopDoor.Lock();
            if (room.BottomDoor) room.BottomDoor.Lock();
        }

        if (enemyManager != null)
        {
            enemyManager.SpawnEnemiesOnEnter();
        }
    }
}

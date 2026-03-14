using UnityEngine;

public class Rooms : MonoBehaviour
{
    [Header("Grid")]
    public Vector2Int GridPosition;

    [Header("Boss Settings")]
    public bool IsBossRoom = false;
    public Transform BossSpawnPoint;
    public GameObject BossPrefab;

    [Header("Room Info")]
    public bool IsStarter = false;
    public bool IsCleared = false;

    [Header("Doors")]
    public Door LeftDoor;
    public Door RightDoor;
    public Door TopDoor;
    public Door BottomDoor;

    [Header("Teleport Points")]
    public Transform LeftPoint;
    public Transform RightPoint;
    public Transform TopPoint;
    public Transform BottomPoint;

    [Header("Enemy Spawns")]
    public Transform[] EnemySpawnPoints;

    private void Start()
    {
        if (IsStarter)
        {
            IsCleared = true;
            UnlockAllDoors();
            return;
        }

        if (EnemySpawnPoints == null || EnemySpawnPoints.Length == 0)
        {
            IsCleared = true;
            UnlockAllDoors();
        }
    }

    void UnlockAllDoors()
    {
        if (LeftDoor != null) LeftDoor.Unlock();
        if (RightDoor != null) RightDoor.Unlock();
        if (TopDoor != null) TopDoor.Unlock();
        if (BottomDoor != null) BottomDoor.Unlock();
    }

    public void OnRoomCleared()
    {
        IsCleared = true;

        if (LeftDoor != null) LeftDoor.Unlock();
        if (RightDoor != null) RightDoor.Unlock();
        if (TopDoor != null) TopDoor.Unlock();
        if (BottomDoor != null) BottomDoor.Unlock();
    }
}
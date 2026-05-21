using UnityEngine;

public class Rooms : MonoBehaviour
{
    [Header("Grid")]
    public Vector2Int GridPosition;
    [Header("Rewards")]
    public GameObject ClosedChestPrefab;
    public GameObject HealthPrefab;
    public GameObject Gear1Prefab;
    public GameObject Gear2Prefab;

    [Header("Reward Spawns")]
    public Transform[] RewardSpawnPoints;
    [HideInInspector] public bool rewardSpawned = false;

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

        SpawnReward();
    }

    private void SpawnReward()
    {
        if (rewardSpawned) return;
        if (IsStarter) return;
        if (RewardSpawnPoints == null || RewardSpawnPoints.Length == 0) return;

        rewardSpawned = true;

        Transform spawnPoint = RewardSpawnPoints[Random.Range(0, RewardSpawnPoints.Length)];

        int roll = Random.Range(0, 2);

        if (roll == 0)
        {
            Instantiate(ClosedChestPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            SpawnDirectReward(spawnPoint.position);
        }
    }

    private void SpawnDirectReward(Vector3 pos)
    {
        int reward = Random.Range(0, 3);

        if (reward == 0)
            Instantiate(HealthPrefab, pos, Quaternion.identity);

        if (reward == 1)
            Instantiate(Gear1Prefab, pos, Quaternion.identity);

        if (reward == 2)
            Instantiate(Gear2Prefab, pos, Quaternion.identity);
    }
}
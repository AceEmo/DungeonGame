using UnityEngine;

public enum RoomType
{
    Normal,
    Starter,
    Blackjack,
    Upgrade,
    Boss,
    Hub
}

public class Rooms : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Vector2Int gridPosition;

    [Header("Rewards")]
    [SerializeField] private GameObject closedChestPrefab;
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private GameObject gear1Prefab;
    [SerializeField] private GameObject gear2Prefab;

    [Header("Reward Spawns")]
    [SerializeField] private Transform[] rewardSpawnPoints;

    [Header("Boss Settings")]
    [SerializeField] private bool isBossRoom = false;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private GameObject bossPrefab;

    [Header("Room Info")]
    [SerializeField] private RoomType roomType;
    [SerializeField] private bool isStarter = false;

    [Header("Doors")]
    [SerializeField] private Door leftDoor;
    [SerializeField] private Door rightDoor;
    [SerializeField] private Door topDoor;
    [SerializeField] private Door bottomDoor;

    [Header("Teleport Points")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;

    [Header("Enemy Spawns")]
    [SerializeField] private Transform[] enemySpawnPoints;

    private bool rewardSpawned;
    private bool isCleared;

    public Vector2Int GridPosition { get => gridPosition; set => gridPosition = value; }
    public RoomType Type => roomType;
    public bool IsStarter { get => isStarter; set => isStarter = value; }
    public bool IsCleared => isCleared;

    public bool IsBossRoom => isBossRoom;
    public Transform BossSpawnPoint => bossSpawnPoint;
    public GameObject BossPrefab => bossPrefab;
    public Transform[] EnemySpawnPoints => enemySpawnPoints;

    public Door LeftDoor => leftDoor;
    public Door RightDoor => rightDoor;
    public Door TopDoor => topDoor;
    public Door BottomDoor => bottomDoor;
    public Transform LeftPoint => leftPoint;
    public Transform RightPoint => rightPoint;
    public Transform TopPoint => topPoint;
    public Transform BottomPoint => bottomPoint;

    public void HideDoor(Vector2Int direction)
    {
        RoomDoorRegistry.HideDoor(this, direction);
    }

    private void Start()
    {
        if (ShouldStartCleared())
        {
            MarkAsCleared();
        }
    }

    public void OnRoomCleared()
    {
        MarkAsCleared();
        SpawnReward();
    }

    public void LockAllDoors()
    {
        if (isStarter)
        {
            return;
        }

        RoomDoorRegistry.LockAllDoors(this);
    }

    private bool ShouldStartCleared()
    {
        return isStarter || enemySpawnPoints == null || enemySpawnPoints.Length == 0;
    }

    private void MarkAsCleared()
    {
        isCleared = true;
        RoomDoorRegistry.UnlockAllDoors(this);
    }

    private void SpawnReward()
    {
        if (rewardSpawned || isStarter || !HasRewardSpawnPoints())
        {
            return;
        }

        rewardSpawned = true;
        Transform spawnPoint = rewardSpawnPoints[Random.Range(0, rewardSpawnPoints.Length)];

        if (Random.Range(0, 2) == 0)
        {
            Instantiate(closedChestPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            RewardSpawner.SpawnRandomReward(spawnPoint.position, healthPrefab, gear1Prefab, gear2Prefab);
        }
    }

    private bool HasRewardSpawnPoints()
    {
        return rewardSpawnPoints != null && rewardSpawnPoints.Length > 0;
    }
}
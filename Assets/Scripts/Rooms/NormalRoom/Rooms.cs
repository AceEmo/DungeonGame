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

    private bool rewardSpawned = false;
    private bool isCleared = false;

    // --- ПУБЛИЧНИ СВОЙСТВА (PROPERTIES) ЗА ВЪНШЕН ДОСТЪП ---
    public Vector2Int GridPosition { get => gridPosition; set => gridPosition = value; }
    public RoomType Type => roomType;
    public bool IsStarter { get => isStarter; set => isStarter = value; }
    public bool IsCleared => isCleared;

    // Тези 4 реда оправят грешката в EnemyManager:
    public bool IsBossRoom => isBossRoom;
    public Transform BossSpawnPoint => bossSpawnPoint;
    public GameObject BossPrefab => bossPrefab;
    public Transform[] EnemySpawnPoints => enemySpawnPoints;

    // Свойства за достъп до вратите и точките (за RoomConnector)
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
        if (direction == Vector2Int.left && leftDoor != null) leftDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.right && rightDoor != null) rightDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.up && topDoor != null) topDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.down && bottomDoor != null) bottomDoor.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (isStarter)
        {
            isCleared = true;
            UnlockAllDoors();
            return;
        }

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            isCleared = true;
            UnlockAllDoors();
        }
    }

    public void OnRoomCleared()
    {
        isCleared = true;
        UnlockAllDoors();
        SpawnReward();
    }

    private void UnlockAllDoors()
    {
        if (leftDoor != null) leftDoor.Unlock();
        if (rightDoor != null) rightDoor.Unlock();
        if (topDoor != null) topDoor.Unlock();
        if (bottomDoor != null) bottomDoor.Unlock();
    }

    public void LockAllDoors()
    {
        if (isStarter) return;

        if (leftDoor != null) leftDoor.Lock();
        if (rightDoor != null) rightDoor.Lock();
        if (topDoor != null) topDoor.Lock();
        if (bottomDoor != null) bottomDoor.Lock();
    }

    private void SpawnReward()
    {
        if (rewardSpawned || isStarter)
        {
            return;
        }

        if (rewardSpawnPoints == null || rewardSpawnPoints.Length == 0)
        {
            return;
        }

        rewardSpawned = true;

        Transform spawnPoint = rewardSpawnPoints[Random.Range(0, rewardSpawnPoints.Length)];
        int roll = Random.Range(0, 2);

        if (roll == 0)
        {
            Instantiate(closedChestPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            SpawnDirectReward(spawnPoint.position);
        }
    }

    private void SpawnDirectReward(Vector3 position)
    {
        int rewardIndex = Random.Range(0, 3);

        switch (rewardIndex)
        {
            case 0:
                Instantiate(healthPrefab, position, Quaternion.identity);
                break;
            case 1:
                Instantiate(gear1Prefab, position, Quaternion.identity);
                break;
            case 2:
                Instantiate(gear2Prefab, position, Quaternion.identity);
                break;
        }
    }
}
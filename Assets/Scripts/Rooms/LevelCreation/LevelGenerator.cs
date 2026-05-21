using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject starterRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject blackjackRoomPrefab;
    public GameObject upgradeRoomPrefab;
    public GameObject[] normalRoomPrefabs;

    [Header("Level Settings")]
    public int roomCount = 10;
    public float roomSize = 30f;

    public static event System.Action<Dictionary<Vector2Int, Rooms>> OnLevelGenerated;
    private readonly Dictionary<Vector2Int, Rooms> rooms = new Dictionary<Vector2Int, Rooms>();
    private LevelLayoutCalculator layoutCalculator;
    private RoomConnector roomConnector;

    private void Awake()
    {
        layoutCalculator = new LevelLayoutCalculator();
        roomConnector = new RoomConnector();
    }

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        rooms.Clear();

        CreateRoom(Vector2Int.zero, starterRoomPrefab, isStarter: true);

        for (int i = 0; i < roomCount - 4; i++)
        {
            Vector2Int randomPos = layoutCalculator.GetRandomAdjacentPosition(rooms.Keys);
            GameObject normalPrefab = normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)];
            CreateRoom(randomPos, normalPrefab, isStarter: false);
        }

        CreateRoom(layoutCalculator.GetRandomAdjacentPosition(rooms.Keys), blackjackRoomPrefab, isStarter: false);
        CreateRoom(layoutCalculator.GetRandomAdjacentPosition(rooms.Keys), upgradeRoomPrefab, isStarter: false);

        Vector2Int bossPos = layoutCalculator.GetBossRoomPosition(rooms);
        CreateRoom(bossPos, bossRoomPrefab, isStarter: false);

        roomConnector.ConnectAllRooms(rooms);
        SetPlayerStart();
        
        OnLevelGenerated?.Invoke(rooms);
    }

    private void CreateRoom(Vector2Int gridPos, GameObject prefab, bool isStarter)
    {
        Vector3 worldPos = new Vector3(gridPos.x * roomSize, gridPos.y * roomSize, 0);
        GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);

        Rooms room = obj.GetComponent<Rooms>();
        room.GridPosition = gridPos;
        room.IsStarter = isStarter;

        rooms.Add(gridPos, room);
    }

    private void SetPlayerStart()
    {
        foreach (var room in rooms.Values)
        {
            if (room.IsStarter)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.transform.position = room.transform.position;
                break;
            }
        }
    }
}
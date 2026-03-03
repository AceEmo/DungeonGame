using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    public GameObject StarterRoomPrefab;
    public GameObject BossRoomPrefab;
    public GameObject BlackjackRoomPrefab;
    public GameObject[] NormalRoomPrefabs;

    [Header("Level Settings")]
    public int RoomCount = 6;
    public float RoomSize = 20f;

    private Dictionary<Vector2Int, Rooms> rooms = new Dictionary<Vector2Int, Rooms>();

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        rooms.Clear();

        CreateRoom(Vector2Int.zero, StarterRoomPrefab, true);

        for (int i = 0; i < RoomCount - 3; i++)
        {
            Vector2Int randomPos = GetRandomAdjacentPosition();
            GameObject normalPrefab = NormalRoomPrefabs[Random.Range(0, NormalRoomPrefabs.Length)];
            CreateRoom(randomPos, normalPrefab, false);
        }

        CreateRoom(GetRandomAdjacentPosition(), BlackjackRoomPrefab, false);

        Vector2Int bossPos = GetSingleAdjacentPosition();
        CreateRoom(bossPos, BossRoomPrefab, false);

        ConnectRooms();
        SetPlayerStart();
    }

    private Vector2Int GetRandomAdjacentPosition()
    {
        List<Vector2Int> possible = new List<Vector2Int>();

        foreach (var room in rooms.Keys)
        {
            possible.Add(room + Vector2Int.up);
            possible.Add(room + Vector2Int.down);
            possible.Add(room + Vector2Int.left);
            possible.Add(room + Vector2Int.right);
        }

        possible.RemoveAll(p => rooms.ContainsKey(p));

        return possible[Random.Range(0, possible.Count)];
    }

    private void CreateRoom(Vector2Int gridPos, GameObject prefab, bool isStarter)
    {
        Vector3 worldPos = new Vector3(gridPos.x * RoomSize, gridPos.y * RoomSize, 0);
        GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);

        Rooms room = obj.GetComponent<Rooms>();
        room.GridPosition = gridPos;
        room.IsStarter = isStarter;

        rooms.Add(gridPos, room);
    }

    private void ConnectRooms()
    {
        foreach (var pair in rooms)
        {
            Vector2Int pos = pair.Key;
            Rooms room = pair.Value;

            ConnectDoor(room, pos, Vector2Int.left);
            ConnectDoor(room, pos, Vector2Int.right);
            ConnectDoor(room, pos, Vector2Int.up);
            ConnectDoor(room, pos, Vector2Int.down);
        }
    }

    private void ConnectDoor(Rooms room, Vector2Int pos, Vector2Int direction)
    {
        Vector2Int neighbourPos = pos + direction;

        if (!rooms.ContainsKey(neighbourPos))
        {
            HideDoor(room, direction);
            return;
        }

        Rooms neighbour = rooms[neighbourPos];

        if (direction == Vector2Int.left)
        {
            room.LeftDoor.TargetRoom = neighbour;
            room.LeftDoor.TargetPoint = neighbour.RightPoint;
        }
        else if (direction == Vector2Int.right)
        {
            room.RightDoor.TargetRoom = neighbour;
            room.RightDoor.TargetPoint = neighbour.LeftPoint;
        }
        else if (direction == Vector2Int.up)
        {
            room.TopDoor.TargetRoom = neighbour;
            room.TopDoor.TargetPoint = neighbour.BottomPoint;
        }
        else if (direction == Vector2Int.down)
        {
            room.BottomDoor.TargetRoom = neighbour;
            room.BottomDoor.TargetPoint = neighbour.TopPoint;
        }
    }

    private void HideDoor(Rooms room, Vector2Int direction)
    {
        if (direction == Vector2Int.left && room.LeftDoor != null)
            room.LeftDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.right && room.RightDoor != null)
            room.RightDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.up && room.TopDoor != null)
            room.TopDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.down && room.BottomDoor != null)
            room.BottomDoor.gameObject.SetActive(false);
    }

    private void SetPlayerStart()
    {
        foreach (var room in rooms.Values)
        {
            if (room.IsStarter)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    Debug.LogError("Player with tag 'Player' not found!");
                    return;
                }
                player.transform.position = room.transform.position;
                break;
            }
        }
    }

    private Vector2Int GetSingleAdjacentPosition()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        foreach (var roomPos in rooms.Keys)
        {
            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (var dir in directions)
            {
                Vector2Int potentialPos = roomPos + dir;

                if (rooms.ContainsKey(potentialPos)) continue;

                int neighborCount = 0;
                foreach (var checkDir in directions)
                {
                    if (rooms.ContainsKey(potentialPos + checkDir))
                        neighborCount++;
                }

                if (neighborCount == 1)
                    candidates.Add(potentialPos);
            }
        }

        if (candidates.Count == 0)
        {
            return Vector2Int.zero;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
}
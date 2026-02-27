using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public GameObject StarterRoomPrefab;
    public GameObject NormalRoomPrefab;
    public GameObject BossRoomPrefab;
    public GameObject BlackjackRoomPrefab;

    public int RoomCount = 6;
    public float RoomSize = 20f;

    private Dictionary<Vector2Int, Rooms> rooms = new Dictionary<Vector2Int, Rooms>();

    private void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        rooms.Clear();

        CreateRoom(Vector2Int.zero, StarterRoomPrefab, true);

        for (int i = 0; i < RoomCount - 3; i++)
        {
            Vector2Int randomPos = GetRandomAdjacentPosition();
            CreateRoom(randomPos, NormalRoomPrefab, false);
        }

        CreateRoom(GetRandomAdjacentPosition(), BossRoomPrefab, false);

        CreateRoom(GetRandomAdjacentPosition(), BlackjackRoomPrefab, false);

        ConnectRooms();
        SetPlayerStart();
    }

    Vector2Int GetRandomAdjacentPosition()
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

    void CreateRoom(Vector2Int gridPos, GameObject prefab, bool isStarter)
    {
        Vector3 worldPos = new Vector3(gridPos.x * RoomSize, gridPos.y * RoomSize, 0);
        GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);

        Rooms room = obj.GetComponent<Rooms>();
        room.GridPosition = gridPos;
        room.IsStarter = isStarter;

        rooms.Add(gridPos, room);
    }

    void ConnectRooms()
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

    void ConnectDoor(Rooms room, Vector2Int pos, Vector2Int direction)
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

    void HideDoor(Rooms room, Vector2Int direction)
    {
        if (direction == Vector2Int.left && room.LeftDoor != null)
            room.LeftDoor.gameObject.SetActive(false);

        if (direction == Vector2Int.right && room.RightDoor != null)
            room.RightDoor.gameObject.SetActive(false);

        if (direction == Vector2Int.up && room.TopDoor != null)
            room.TopDoor.gameObject.SetActive(false);

        if (direction == Vector2Int.down && room.BottomDoor != null)
            room.BottomDoor.gameObject.SetActive(false);
    }

    void SetPlayerStart()
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
}
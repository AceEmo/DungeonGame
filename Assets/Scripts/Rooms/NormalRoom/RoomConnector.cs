using UnityEngine;
using System.Collections.Generic;

public class RoomConnector
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up,
        Vector2Int.down
    };

    public void ConnectAllRooms(Dictionary<Vector2Int, Rooms> rooms)
    {
        foreach (var pair in rooms)
        {
            ConnectRoomDoors(pair.Key, pair.Value, rooms);
        }
    }

    private void ConnectRoomDoors(Vector2Int position, Rooms room, Dictionary<Vector2Int, Rooms> rooms)
    {
        foreach (Vector2Int direction in Directions)
        {
            ConnectDoor(room, position, direction, rooms);
        }
    }

    private void ConnectDoor(Rooms room, Vector2Int position, Vector2Int direction, Dictionary<Vector2Int, Rooms> rooms)
    {
        Vector2Int neighbourPosition = position + direction;

        if (!rooms.TryGetValue(neighbourPosition, out Rooms neighbour))
        {
            RoomDoorRegistry.HideDoor(room, direction);
            return;
        }

        RoomDoorRegistry.ConnectDoors(room, neighbour, direction);
    }
}
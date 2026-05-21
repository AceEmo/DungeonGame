using UnityEngine;
using System.Collections.Generic;

public class RoomConnector
{
    public void ConnectAllRooms(Dictionary<Vector2Int, Rooms> rooms)
    {
        foreach (var pair in rooms)
        {
            Vector2Int pos = pair.Key;
            Rooms room = pair.Value;

            ConnectDoor(room, pos, Vector2Int.left, rooms);
            ConnectDoor(room, pos, Vector2Int.right, rooms);
            ConnectDoor(room, pos, Vector2Int.up, rooms);
            ConnectDoor(room, pos, Vector2Int.down, rooms);
        }
    }

    private void ConnectDoor(Rooms room, Vector2Int pos, Vector2Int direction, Dictionary<Vector2Int, Rooms> rooms)
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
        if (direction == Vector2Int.left && room.LeftDoor != null) room.LeftDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.right && room.RightDoor != null) room.RightDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.up && room.TopDoor != null) room.TopDoor.gameObject.SetActive(false);
        else if (direction == Vector2Int.down && room.BottomDoor != null) room.BottomDoor.gameObject.SetActive(false);
    }
}
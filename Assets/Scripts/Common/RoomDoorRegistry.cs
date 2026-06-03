using UnityEngine;

public readonly struct RoomDoorEntry
{
    public readonly Door Door;
    public readonly Transform TeleportPoint;

    public RoomDoorEntry(Door door, Transform teleportPoint)
    {
        Door = door;
        TeleportPoint = teleportPoint;
    }
}

public static class RoomDoorRegistry
{
    public static RoomDoorEntry GetEntry(Rooms room, Vector2Int direction)
    {
        if (direction == Vector2Int.left)
            return new RoomDoorEntry(room.LeftDoor, room.LeftPoint);
        if (direction == Vector2Int.right)
            return new RoomDoorEntry(room.RightDoor, room.RightPoint);
        if (direction == Vector2Int.up)
            return new RoomDoorEntry(room.TopDoor, room.TopPoint);
        if (direction == Vector2Int.down)
            return new RoomDoorEntry(room.BottomDoor, room.BottomPoint);

        return default;
    }

    public static void HideDoor(Rooms room, Vector2Int direction)
    {
        RoomDoorEntry entry = GetEntry(room, direction);
        if (entry.Door != null)
        {
            entry.Door.gameObject.SetActive(false);
        }
    }

    public static void ConnectDoors(Rooms room, Rooms neighbour, Vector2Int direction)
    {
        RoomDoorEntry source = GetEntry(room, direction);
        RoomDoorEntry target = GetEntry(neighbour, OppositeDirection(direction));

        if (source.Door == null)
        {
            return;
        }

        source.Door.TargetRoom = neighbour;
        source.Door.TargetPoint = target.TeleportPoint;
    }

    public static Vector2Int OppositeDirection(Vector2Int direction)
    {
        return -direction;
    }

    public static void UnlockAllDoors(Rooms room)
    {
        UnlockDoor(room.LeftDoor);
        UnlockDoor(room.RightDoor);
        UnlockDoor(room.TopDoor);
        UnlockDoor(room.BottomDoor);
    }

    public static void LockAllDoors(Rooms room)
    {
        LockDoor(room.LeftDoor);
        LockDoor(room.RightDoor);
        LockDoor(room.TopDoor);
        LockDoor(room.BottomDoor);
    }

    private static void UnlockDoor(Door door)
    {
        if (door != null)
        {
            door.Unlock();
        }
    }

    private static void LockDoor(Door door)
    {
        if (door != null)
        {
            door.Lock();
        }
    }
}

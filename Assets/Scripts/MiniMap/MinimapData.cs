using System.Collections.Generic;
using UnityEngine;

public class MinimapData
{
    public Dictionary<Vector2Int, RoomType> RoomTypes { get; } = new Dictionary<Vector2Int, RoomType>();
    public HashSet<Vector2Int> ExploredRooms { get; } = new HashSet<Vector2Int>();

    public void Clear()
    {
        RoomTypes.Clear();
        ExploredRooms.Clear();
    }

    public void AddRoom(Vector2Int position, RoomType type)
    {
        RoomTypes[position] = type;
    }

    public void MarkAsExplored(Vector2Int position)
    {
        ExploredRooms.Add(position);
    }

    public bool IsExplored(Vector2Int position)
    {
        return ExploredRooms.Contains(position);
    }

    public bool IsNeighborOf(Vector2Int target, Vector2Int center)
    {
        return (target - center).sqrMagnitude == 1;
    }

    public bool IsAnyNeighborExplored(Vector2Int position)
    {
        foreach (Vector2Int explored in ExploredRooms)
        {
            if (IsNeighborOf(position, explored))
            {
                return true;
            }
        }
        return false;
    }

    public List<Vector2Int> GetKnownRooms()
    {
        List<Vector2Int> knownRooms = new List<Vector2Int>();
        foreach (Vector2Int position in RoomTypes.Keys)
        {
            if (IsExplored(position) || IsAnyNeighborExplored(position))
            {
                knownRooms.Add(position);
            }
        }
        return knownRooms;
    }
}
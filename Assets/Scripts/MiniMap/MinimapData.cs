using System.Collections.Generic;
using UnityEngine;

public class MinimapData
{
    public Dictionary<Vector2Int, RoomType> RoomTypes { get; } = new Dictionary<Vector2Int, RoomType>();
    public HashSet<Vector2Int> ExploredRooms { get; } = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> DiscoveredRooms { get; } = new HashSet<Vector2Int>();

    public void Clear()
    {
        RoomTypes.Clear();
        ExploredRooms.Clear();
        DiscoveredRooms.Clear();
    }

    public void AddRoom(Vector2Int position, RoomType type)
    {
        RoomTypes[position] = type;
    }

    public void MarkAsExplored(Vector2Int position)
    {
        ExploredRooms.Add(position);
        DiscoverNeighbors(position);
    }

    public bool IsExplored(Vector2Int position)
    {
        return ExploredRooms.Contains(position);
    }

    public bool IsDiscovered(Vector2Int position)
    {
        return DiscoveredRooms.Contains(position);
    }

    public bool IsNeighborOf(Vector2Int target, Vector2Int center)
    {
        return (target - center).sqrMagnitude == 1;
    }

    private void DiscoverNeighbors(Vector2Int center)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighborPos = center + dir;
            if (RoomTypes.ContainsKey(neighborPos))
            {
                DiscoveredRooms.Add(neighborPos);
            }
        }
    }

    public List<Vector2Int> GetKnownRooms()
    {
        List<Vector2Int> knownRooms = new List<Vector2Int>();
        foreach (Vector2Int position in RoomTypes.Keys)
        {
            if (IsExplored(position) || IsDiscovered(position))
            {
                knownRooms.Add(position);
            }
        }
        return knownRooms;
    }

    public void InitializeDefaultHubState()
    {
        Clear();

        Vector2Int centerPos = Vector2Int.zero;
        
        AddRoom(centerPos, RoomType.Normal);
        MarkAsExplored(centerPos);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int dir in directions)
        {
            AddRoom(centerPos + dir, RoomType.Normal);
        }
    }
}
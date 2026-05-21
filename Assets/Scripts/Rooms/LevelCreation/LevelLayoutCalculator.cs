using UnityEngine;
using System.Collections.Generic;

public class LevelLayoutCalculator
{
    private readonly Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public Vector2Int GetRandomAdjacentPosition(ICollection<Vector2Int> existingPositions)
    {
        List<Vector2Int> possible = new List<Vector2Int>();

        foreach (var pos in existingPositions)
        {
            possible.Add(pos + Vector2Int.up);
            possible.Add(pos + Vector2Int.down);
            possible.Add(pos + Vector2Int.left);
            possible.Add(pos + Vector2Int.right);
        }

        possible.RemoveAll(existingPositions.Contains);

        return possible[Random.Range(0, possible.Count)];
    }

    public Vector2Int GetBossRoomPosition(Dictionary<Vector2Int, Rooms> rooms)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();

        foreach (var roomPos in rooms.Keys)
        {
            foreach (var dir in directions)
            {
                Vector2Int potentialPos = roomPos + dir;

                if (rooms.ContainsKey(potentialPos)) continue;

                if (HasExactlyOneNeighbor(potentialPos, rooms))
                {
                    candidates.Add(potentialPos);
                }
            }
        }

        if (candidates.Count == 0)
        {
            return GetRandomAdjacentPosition(rooms.Keys);
        }

        return GetFurthestPosition(candidates, Vector2Int.zero);
    }

    private bool HasExactlyOneNeighbor(Vector2Int position, Dictionary<Vector2Int, Rooms> rooms)
    {
        int neighborCount = 0;
        foreach (var checkDir in directions)
        {
            if (rooms.ContainsKey(position + checkDir))
            {
                neighborCount++;
            }
        }
        return neighborCount == 1;
    }

    private Vector2Int GetFurthestPosition(List<Vector2Int> positions, Vector2Int fromPosition)
    {
        Vector2Int furthestPos = positions[0];
        float maxDistance = 0f;

        foreach (var pos in positions)
        {
            float currentDistance = Mathf.Abs(pos.x - fromPosition.x) + Mathf.Abs(pos.y - fromPosition.y);

            if (currentDistance > maxDistance)
            {
                maxDistance = currentDistance;
                furthestPos = pos;
            }
        }

        return furthestPos;
    }
}
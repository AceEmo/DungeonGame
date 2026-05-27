using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class LevelLayoutCalculatorTests
{
    private LevelLayoutCalculator _calculator;

    [SetUp]
    public void SetUp()
    {
        _calculator = new LevelLayoutCalculator();
    }

    [Test]
    public void GetRandomAdjacentPosition_ShouldReturnPositionNotInsideExistingCollection()
    {
        HashSet<Vector2Int> existingPositions = new HashSet<Vector2Int> { Vector2Int.zero };

        Vector2Int result = _calculator.GetRandomAdjacentPosition(existingPositions);

        Assert.AreNotEqual(Vector2Int.zero, result);
        Assert.IsTrue(Mathf.Abs(result.x) + Mathf.Abs(result.y) == 1);
    }

    [Test]
    public void GetBossRoomPosition_WhenNoCandidatesWithOneNeighbor_ShouldReturnAnyAdjacent()
    {
        Dictionary<Vector2Int, Rooms> rooms = new Dictionary<Vector2Int, Rooms>();
        GameObject holder = new GameObject();
        rooms.Add(Vector2Int.zero, holder.AddComponent<Rooms>());

        Vector2Int result = _calculator.GetBossRoomPosition(rooms);

        Assert.AreNotEqual(Vector2Int.zero, result);
        Object.DestroyImmediate(holder);
    }
}
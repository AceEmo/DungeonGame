using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class MinimapDataTests
{
    private MinimapData _mapData;

    [SetUp]
    public void SetUp()
    {
        _mapData = new MinimapData();
    }

    [Test]
    public void Clear_ShouldEmptyAllCollections()
    {
        _mapData.AddRoom(Vector2Int.zero, RoomType.Normal);
        _mapData.MarkAsExplored(Vector2Int.zero);

        _mapData.Clear();

        Assert.AreEqual(0, _mapData.RoomTypes.Count);
        Assert.AreEqual(0, _mapData.ExploredRooms.Count);
        Assert.AreEqual(0, _mapData.DiscoveredRooms.Count);
    }

    [Test]
    public void MarkAsExplored_ShouldExplorRoomAndDiscoverValidNeighbors()
    {
        _mapData.AddRoom(Vector2Int.zero, RoomType.Starter);
        _mapData.AddRoom(Vector2Int.up, RoomType.Boss);

        _mapData.MarkAsExplored(Vector2Int.zero);

        Assert.IsTrue(_mapData.IsExplored(Vector2Int.zero));
        Assert.IsTrue(_mapData.IsDiscovered(Vector2Int.up));
        Assert.IsFalse(_mapData.IsDiscovered(Vector2Int.down));
    }

    [Test]
    public void GetKnownRooms_ShouldReturnExploredAndDiscoveredRoomsOnly()
    {
        _mapData.AddRoom(Vector2Int.zero, RoomType.Normal);
        _mapData.AddRoom(Vector2Int.right, RoomType.Blackjack);
        _mapData.AddRoom(new Vector2Int(5, 5), RoomType.Upgrade);
        _mapData.MarkAsExplored(Vector2Int.zero);

        List<Vector2Int> known = _mapData.GetKnownRooms();

        Assert.Contains(Vector2Int.zero, known);
        Assert.Contains(Vector2Int.right, known);
        Assert.IsFalse(known.Contains(new Vector2Int(5, 5)));
    }

    [Test]
    public void InitializeDefaultHubState_ShouldSetupHubGridCorrectly()
    {
        _mapData.InitializeDefaultHubState();

        Assert.AreEqual(5, _mapData.RoomTypes.Count);
        Assert.IsTrue(_mapData.IsExplored(Vector2Int.zero));
    }
}
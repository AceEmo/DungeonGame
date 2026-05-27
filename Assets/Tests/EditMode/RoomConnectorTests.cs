using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class RoomConnectorTests
{
    private RoomConnector _connector;
    private Dictionary<Vector2Int, Rooms> _roomMap;
    private GameObject _roomObj1;
    private GameObject _roomObj2;

    [SetUp]
    public void SetUp()
    {
        _connector = new RoomConnector();
        _roomMap = new Dictionary<Vector2Int, Rooms>();

        _roomObj1 = new GameObject();
        _roomObj2 = new GameObject();

        Rooms r1 = _roomObj1.AddComponent<Rooms>();
        Rooms r2 = _roomObj2.AddComponent<Rooms>();

        SetupMockDoorsAndPoints(r1);
        SetupMockDoorsAndPoints(r2);

        _roomMap.Add(Vector2Int.zero, r1);
        _roomMap.Add(Vector2Int.right, r2);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_roomObj1);
        Object.DestroyImmediate(_roomObj2);
    }

    private void SetupMockDoorsAndPoints(Rooms room)
    {
        var leftDoorObj = new GameObject();
        var rightDoorObj = new GameObject();
        var topDoorObj = new GameObject();
        var bottomDoorObj = new GameObject();

        typeof(Rooms).GetField("leftDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(room, leftDoorObj.AddComponent<Door>());
        typeof(Rooms).GetField("rightDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(room, rightDoorObj.AddComponent<Door>());
        typeof(Rooms).GetField("topDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(room, topDoorObj.AddComponent<Door>());
        typeof(Rooms).GetField("bottomDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(room, bottomDoorObj.AddComponent<Door>());

        typeof(Rooms).GetField("leftPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(room, new GameObject().transform);
        typeof(Rooms).GetField("rightPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(room, new GameObject().transform);
        typeof(Rooms).GetField("topPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(room, new GameObject().transform);
        typeof(Rooms).GetField("bottomPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(room, new GameObject().transform);
    }

    [Test]
    public void ConnectAllRooms_ShouldEstablishDoorTargetsForValidNeighbors()
    {
        _connector.ConnectAllRooms(_roomMap);

        Assert.AreEqual(_roomMap[Vector2Int.right], _roomMap[Vector2Int.zero].RightDoor.TargetRoom);
        Assert.AreEqual(_roomMap[Vector2Int.zero].RightPoint, _roomMap[Vector2Int.right].LeftDoor.TargetPoint);
    }
}
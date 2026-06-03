using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class RoomDoorRegistryTests
{
    private GameObject _roomObject;
    private Rooms _room;

    [SetUp]
    public void SetUp()
    {
        _roomObject = new GameObject("Room");
        _room = _roomObject.AddComponent<Rooms>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_roomObject);
    }

    [Test]
    public void OppositeDirection_ShouldInvertVector()
    {
        Assert.AreEqual(Vector2Int.right, RoomDoorRegistry.OppositeDirection(Vector2Int.left));
        Assert.AreEqual(Vector2Int.down, RoomDoorRegistry.OppositeDirection(Vector2Int.up));
    }

    [Test]
    public void HideDoor_WithMissingDoor_ShouldNotThrow()
    {
        Assert.DoesNotThrow(() => RoomDoorRegistry.HideDoor(_room, Vector2Int.left));
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RoomsAndTriggersTests
{
    private GameObject _roomContainer;
    private Rooms _room;
    private GameObject _player;

    [SetUp]
    public void SetUp()
    {
        _roomContainer = new GameObject();
        _room = _roomContainer.AddComponent<Rooms>();
        
        var leftDoor = new GameObject();
        typeof(Rooms).GetField("leftDoor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_room, leftDoor.AddComponent<Door>());

        _player = new GameObject("Player");
        _player.tag = "Player";
        _player.AddComponent<BoxCollider2D>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_roomContainer);
        Object.Destroy(_player);
    }

    [UnityTest]
    public IEnumerator Start_WhenIsStarterRoom_ShouldMarkAsCleared()
    {
        _room.IsStarter = true;
        
        _room.SendMessage("Start");
        yield return null;

        Assert.IsTrue(_room.IsCleared);
    }

    [UnityTest]
    public IEnumerator LockAllDoors_ShouldCallLockOnAttachedDoors()
    {
        _room.IsStarter = false;
        _room.SendMessage("Start");
        yield return null;

        _room.LockAllDoors();

        Assert.IsFalse(_room.LeftDoor.OpenDoor.activeSelf);
    }

    [UnityTest]
    public IEnumerator RoomTrigger_OnTriggerEnter2D_ShouldLockDoors()
    {
        _room.IsStarter = false;
        GameObject triggerObj = new GameObject();
        triggerObj.transform.SetParent(_roomContainer.transform);
        var trigger = triggerObj.AddComponent<RoomTrigger>();
        triggerObj.AddComponent<BoxCollider2D>();

        trigger.SendMessage("Awake");
        var collider = _player.GetComponent<BoxCollider2D>();
        trigger.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        Assert.IsFalse(_room.LeftDoor.OpenDoor.activeSelf);
    }
}
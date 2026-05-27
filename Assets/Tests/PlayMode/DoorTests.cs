using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DoorTests
{
    private GameObject _doorObject;
    private GameObject _playerObject;
    private Door _door;
    private GameObject _closedVisual;
    private GameObject _openVisual;

    [SetUp]
    public void SetUp()
    {
        _doorObject = new GameObject();
        _door = _doorObject.AddComponent<Door>();

        _closedVisual = new GameObject();
        _openVisual = new GameObject();

        _door.ClosedDoor = _closedVisual;
        _door.OpenDoor = _openVisual;

        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";
        _playerObject.AddComponent<BoxCollider2D>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_doorObject);
        Object.Destroy(_playerObject);
        Object.Destroy(_closedVisual);
        Object.Destroy(_openVisual);
    }

    [UnityTest]
    public IEnumerator LockAndUnlock_ShouldToggleVisualsCorrectly()
    {
        _door.Lock();
        Assert.IsTrue(_closedVisual.activeSelf);
        Assert.IsFalse(_openVisual.activeSelf);

        _door.Unlock();
        Assert.IsFalse(_closedVisual.activeSelf);
        Assert.IsTrue(_openVisual.activeSelf);

        yield return null;
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WhenLocked_ShouldNotTeleportPlayer()
    {
        _door.Lock();
        GameObject targetPoint = new GameObject();
        _door.TargetPoint = targetPoint.transform;
        _door.TargetRoom = _doorObject.AddComponent<Rooms>();
        _playerObject.transform.position = Vector3.zero;

        var collider = _playerObject.GetComponent<BoxCollider2D>();
        _door.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        Assert.AreEqual(Vector3.zero, _playerObject.transform.position);
        Object.Destroy(targetPoint);
    }
}
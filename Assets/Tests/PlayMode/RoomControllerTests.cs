using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Cinemachine;

public class RoomControllerTests
{
    private GameObject _roomObject;
    private GameObject _cameraObject;
    private GameObject _playerObject;
    private RoomController _roomController;
    private CinemachineCamera _cinemachineCamera;

    [SetUp]
    public void SetUp()
    {
        _roomObject = new GameObject("Room");
        _roomObject.AddComponent<BoxCollider2D>();

        _cameraObject = new GameObject("CinemachineCamera");
        _cinemachineCamera = _cameraObject.AddComponent<CinemachineCamera>();

        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";
        _playerObject.AddComponent<BoxCollider2D>();

        _roomController = _roomObject.AddComponent<RoomController>();
        _roomController.roomCamera = _cinemachineCamera;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_roomObject);
        Object.Destroy(_cameraObject);
        Object.Destroy(_playerObject);
    }

    [UnityTest]
    public IEnumerator Start_ShouldSetCameraPriorityToZero()
    {
        _roomController.isBossRoom = false;

        yield return null;

        Assert.AreEqual(0, _cinemachineCamera.Priority);
    }

    [UnityTest]
    public IEnumerator Start_WhenIsBossRoom_ShouldSetupBossCameraTargets()
    {
        _roomController.isBossRoom = true;

        yield return null;

        Assert.AreEqual(_playerObject.transform, _cinemachineCamera.Follow);
        Assert.AreEqual(_playerObject.transform, _cinemachineCamera.LookAt);
    }

    [UnityTest]
    public IEnumerator Start_WhenIsBossRoom_ShouldConfigureConfinerBoundingShape()
    {
        _roomController.isBossRoom = true;
        var confiner = _cameraObject.AddComponent<CinemachineConfiner2D>();
        var boxCollider = _roomObject.GetComponent<BoxCollider2D>();

        yield return null;

        Assert.AreEqual(boxCollider, confiner.BoundingShape2D);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WhenPlayerEnters_ShouldRaiseCameraPriority()
    {
        _roomController.isBossRoom = false;
        yield return null;

        var playerCollider = _playerObject.GetComponent<BoxCollider2D>();
        _roomController.SendMessage("OnTriggerEnter2D", playerCollider);

        Assert.AreEqual(10, _cinemachineCamera.Priority);
    }

    [UnityTest]
    public IEnumerator OnTriggerExit2D_WhenPlayerExits_ShouldLowerCameraPriority()
    {
        _roomController.isBossRoom = false;
        yield return null;

        var playerCollider = _playerObject.GetComponent<BoxCollider2D>();
        _roomController.SendMessage("OnTriggerEnter2D", playerCollider);
        _roomController.SendMessage("OnTriggerExit2D", playerCollider);

        Assert.AreEqual(0, _cinemachineCamera.Priority);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WhenNonPlayerEnters_ShouldNotChangePriority()
    {
        _roomController.isBossRoom = false;
        yield return null;

        GameObject nonPlayer = new GameObject("Enemy");
        nonPlayer.tag = "Untagged";
        var enemyCollider = nonPlayer.AddComponent<BoxCollider2D>();

        _roomController.SendMessage("OnTriggerEnter2D", enemyCollider);

        Assert.AreEqual(0, _cinemachineCamera.Priority);

        Object.Destroy(nonPlayer);
    }
}
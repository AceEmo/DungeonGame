using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Cinemachine;

public class RoomControllerTests
{
    private GameObject roomObject;
    private RoomController roomController;
    private GameObject cameraObject;
    private CinemachineCamera cinemachineCamera;
    private GameObject playerObject;
    private BoxCollider2D playerCollider;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;
        
        playerObject = new GameObject("PlayerTest");
        playerObject.SetActive(false);
        playerObject.tag = "Player";
        playerCollider = playerObject.AddComponent<BoxCollider2D>();

        cameraObject = new GameObject("RoomCamera");
        cameraObject.SetActive(false);
        cinemachineCamera = cameraObject.AddComponent<CinemachineCamera>();

        roomObject = new GameObject("Room");
        roomObject.SetActive(false);
        roomController = roomObject.AddComponent<RoomController>();
        
        roomController.roomCamera = cinemachineCamera;

        playerObject.SetActive(true);
        cameraObject.SetActive(true);
        roomObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(roomObject);
        Object.DestroyImmediate(cameraObject);
        
        foreach (var boss in Object.FindObjectsByType<Boss>(FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(boss.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator StartSetsPriorityToZero()
    {
        cinemachineCamera.Priority = 10;
        roomController.isBossRoom = false;

        MethodInfo startMethod = typeof(RoomController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(roomController, null);

        Assert.AreEqual(0, cinemachineCamera.Priority.Value);

        yield return null;
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2DWithPlayerSetsPriorityToTen()
    {
        MethodInfo onTriggerEnterMethod = typeof(RoomController).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        
        onTriggerEnterMethod.Invoke(roomController, new object[] { playerCollider });

        Assert.AreEqual(10, cinemachineCamera.Priority.Value);

        yield return null;
    }

    [UnityTest]
    public IEnumerator OnTriggerExit2DWithPlayerSetsPriorityToZero()
    {
        cinemachineCamera.Priority = 10;
        MethodInfo onTriggerExitMethod = typeof(RoomController).GetMethod("OnTriggerExit2D", BindingFlags.NonPublic | BindingFlags.Instance);
        
        onTriggerExitMethod.Invoke(roomController, new object[] { playerCollider });

        Assert.AreEqual(0, cinemachineCamera.Priority.Value);

        yield return null;
    }

    [UnityTest]
    public IEnumerator StartWithBossRoomSetsBossCameraProperties()
    {
        roomController.isBossRoom = true;
        BoxCollider2D roomCollider = roomObject.AddComponent<BoxCollider2D>();
        
        CinemachineConfiner2D confiner = cameraObject.AddComponent<CinemachineConfiner2D>();

        MethodInfo startMethod = typeof(RoomController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(roomController, null);

        Assert.AreEqual(playerObject.transform, cinemachineCamera.Follow);
        Assert.AreEqual(playerObject.transform, cinemachineCamera.LookAt);
        Assert.AreEqual(roomCollider, confiner.BoundingShape2D);

        yield return null;
    }
}
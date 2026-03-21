using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DoorTests
{
    private GameObject doorObject;
    private Door door;
    private GameObject closedDoor;
    private GameObject openDoor;
    private GameObject playerObject;
    private GameObject targetPoint;
    private GameObject dummyRoomObject;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;

        doorObject = new GameObject("Door");
        doorObject.SetActive(false);
        door = doorObject.AddComponent<Door>();

        closedDoor = new GameObject("ClosedDoor");
        openDoor = new GameObject("OpenDoor");
        door.ClosedDoor = closedDoor;
        door.OpenDoor = openDoor;

        playerObject = new GameObject("PlayerTest");
        playerObject.tag = "Player";
        playerObject.AddComponent<Rigidbody2D>();
        playerObject.AddComponent<BoxCollider2D>();

        targetPoint = new GameObject("Target");
        targetPoint.transform.position = new Vector3(10f, 10f, 0f);
        
        dummyRoomObject = new GameObject("DummyRoom");
        Rooms dummyRoom = dummyRoomObject.AddComponent<Rooms>();

        door.TargetPoint = targetPoint.transform;
        door.TargetRoom = dummyRoom;

        doorObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(doorObject);
        Object.DestroyImmediate(closedDoor);
        Object.DestroyImmediate(openDoor);
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(targetPoint);
        Object.DestroyImmediate(dummyRoomObject);
        
        foreach (var boss in Object.FindObjectsByType<Boss>(FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(boss.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator LockDisablesOpenDoorAndEnablesClosedDoor()
    {
        door.Lock();

        Assert.IsTrue(closedDoor.activeSelf);
        Assert.IsFalse(openDoor.activeSelf);

        yield return null;
    }

    [UnityTest]
    public IEnumerator UnlockEnablesOpenDoorAndDisablesClosedDoor()
    {
        door.Unlock();

        Assert.IsFalse(closedDoor.activeSelf);
        Assert.IsTrue(openDoor.activeSelf);

        yield return null;
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2DTeleportsPlayerIfUnlocked()
    {
        door.Unlock();
        
        MethodInfo onTriggerEnterMethod = typeof(Door).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        onTriggerEnterMethod.Invoke(door, new object[] { playerObject.GetComponent<BoxCollider2D>() });

        Assert.AreEqual(new Vector3(10f, 10f, 0f), playerObject.transform.position);

        yield return null;
    }
}
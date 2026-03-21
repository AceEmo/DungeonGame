using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RoomsTests
{
    private GameObject roomObject;
    private Rooms room;
    private List<GameObject> mockPrefabs;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;
        mockPrefabs = new List<GameObject>();

        roomObject = new GameObject("Room");
        roomObject.SetActive(false);
        room = roomObject.AddComponent<Rooms>();

        GameObject chestPrefab = new GameObject("ChestPrefab");
        GameObject healthPrefab = new GameObject("HealthPrefab");
        GameObject gear1Prefab = new GameObject("Gear1Prefab");
        GameObject gear2Prefab = new GameObject("Gear2Prefab");
        
        mockPrefabs.Add(chestPrefab);
        mockPrefabs.Add(healthPrefab);
        mockPrefabs.Add(gear1Prefab);
        mockPrefabs.Add(gear2Prefab);

        room.ClosedChestPrefab = chestPrefab;
        room.HealthPrefab = healthPrefab;
        room.Gear1Prefab = gear1Prefab;
        room.Gear2Prefab = gear2Prefab;

        GameObject spawnPoint = new GameObject("SpawnPoint");
        spawnPoint.transform.SetParent(roomObject.transform);
        room.RewardSpawnPoints = new Transform[] { spawnPoint.transform };

        room.LeftDoor = new GameObject("LeftDoor").AddComponent<Door>();
        room.LeftDoor.ClosedDoor = new GameObject("Closed");
        room.LeftDoor.OpenDoor = new GameObject("Open");

        roomObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(roomObject);
        foreach (GameObject mock in mockPrefabs)
        {
            Object.DestroyImmediate(mock);
        }

        foreach (var boss in Object.FindObjectsByType<Boss>(FindObjectsSortMode.None))
        {
            Object.DestroyImmediate(boss.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator StarterRoomUnlocksDoorsAndSetsClearedOnStart()
    {
        room.IsStarter = true;
        
        yield return null; 

        Assert.IsTrue(room.IsCleared);
        Assert.IsTrue(room.LeftDoor.OpenDoor.activeSelf);
    }

    [UnityTest]
    public IEnumerator OnRoomClearedUnlocksDoorsAndSpawnsReward()
    {
        room.IsStarter = false;

        room.OnRoomCleared();

        yield return null;

        Assert.IsTrue(room.IsCleared);
        Assert.IsTrue(room.rewardSpawned);
        Assert.IsTrue(room.LeftDoor.OpenDoor.activeSelf);
    }
}
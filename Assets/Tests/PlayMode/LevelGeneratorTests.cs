using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelGeneratorTests
{
    private GameObject generatorObject;
    private LevelGenerator levelGenerator;
    private GameObject mockStarter;
    private GameObject mockNormal;
    private GameObject mockBoss;
    private GameObject mockBlackjack;
    private GameObject playerObject;

    [SetUp]
    public void Setup()
    {
        generatorObject = new GameObject("Generator");
        generatorObject.SetActive(false);
        levelGenerator = generatorObject.AddComponent<LevelGenerator>();

        mockStarter = CreateMockRoomPrefab("Starter");
        mockNormal = CreateMockRoomPrefab("Normal");
        mockBoss = CreateMockRoomPrefab("Boss");
        mockBlackjack = CreateMockRoomPrefab("Blackjack");

        levelGenerator.starterRoomPrefab = mockStarter;
        levelGenerator.normalRoomPrefabs = new GameObject[] { mockNormal };
        levelGenerator.bossRoomPrefab = mockBoss;
        levelGenerator.blackjackRoomPrefab = mockBlackjack;
        levelGenerator.roomCount = 5;

        playerObject = new GameObject("PlayerTest");
        playerObject.tag = "Player";

        generatorObject.SetActive(true);
    }

    private GameObject CreateMockRoomPrefab(string name)
    {
        GameObject obj = new GameObject(name);
        Rooms room = obj.AddComponent<Rooms>();
        
        Door door = obj.AddComponent<Door>();
        door.ClosedDoor = new GameObject("Closed");
        door.OpenDoor = new GameObject("Open");
        
        room.LeftDoor = door;
        room.RightDoor = door;
        room.TopDoor = door;
        room.BottomDoor = door;

        obj.SetActive(false);
        return obj;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(generatorObject);
        Object.DestroyImmediate(mockStarter);
        Object.DestroyImmediate(mockNormal);
        Object.DestroyImmediate(mockBoss);
        Object.DestroyImmediate(mockBlackjack);
        Object.DestroyImmediate(playerObject);

        Rooms[] remainingRooms = Object.FindObjectsByType<Rooms>(FindObjectsSortMode.None);
        foreach (var r in remainingRooms)
        {
            Object.DestroyImmediate(r.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator GenerateLevelCreatesCorrectNumberOfRooms()
    {
        yield return null;

        FieldInfo roomsField = typeof(LevelGenerator).GetField("rooms", BindingFlags.NonPublic | BindingFlags.Instance);
        var generatedRooms = (Dictionary<Vector2Int, Rooms>)roomsField.GetValue(levelGenerator);

        Assert.AreEqual(5, generatedRooms.Count);
    }

    [UnityTest]
    public IEnumerator GenerateLevelPlacesPlayerInStarterRoom()
    {
        yield return null;

        FieldInfo roomsField = typeof(LevelGenerator).GetField("rooms", BindingFlags.NonPublic | BindingFlags.Instance);
        var generatedRooms = (Dictionary<Vector2Int, Rooms>)roomsField.GetValue(levelGenerator);

        Rooms starterRoom = null;
        foreach (var room in generatedRooms.Values)
        {
            if (room.IsStarter)
            {
                starterRoom = room;
                break;
            }
        }

        Assert.IsNotNull(starterRoom);
        Assert.AreEqual(starterRoom.transform.position, playerObject.transform.position);
    }
}
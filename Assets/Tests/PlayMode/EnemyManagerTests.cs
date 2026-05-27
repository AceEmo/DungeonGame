using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyManagerTests
{
    private GameObject _container;
    private EnemyManager _enemyManager;
    private Rooms _room;

    [SetUp]
    public void SetUp()
    {
        _container = new GameObject();
        _room = _container.AddComponent<Rooms>();
        _enemyManager = _container.AddComponent<EnemyManager>();

        typeof(EnemyManager).GetField("room", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_enemyManager, _room);
        typeof(EnemyManager).GetField("enemyPrefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_enemyManager, new GameObject[0]);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_container);
    }

    [UnityTest]
    public IEnumerator SpawnEnemiesOnEnter_WhenNoEnemiesConfigured_ShouldClearRoomImmediately()
    {
        _enemyManager.SendMessage("Awake");
        typeof(EnemyManager).GetField("initialSpawnDelay", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_enemyManager, 0f);

        _enemyManager.SpawnEnemiesOnEnter();
        yield return null;

        Assert.IsTrue(_room.IsCleared);
    }
}
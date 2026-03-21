using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyManagerTests
{
    private GameObject managerObject;
    private EnemyManager enemyManager;
    private GameObject roomObject;
    private Rooms room;
    private GameObject enemyPrefab;
    private GameObject bossPrefab;
    private GameObject playerObject;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;

        playerObject = new GameObject("Player");
        playerObject.tag = "Player";

        roomObject = new GameObject("Room");
        roomObject.SetActive(false);
        room = roomObject.AddComponent<Rooms>();

        enemyPrefab = new GameObject("EnemyPrefab");
        enemyPrefab.SetActive(false);
        Enemy enemyComp = enemyPrefab.AddComponent<Enemy>();
        enemyComp.data = ScriptableObject.CreateInstance<EnemyData>();
        enemyPrefab.AddComponent<EnemyHealth>();

        bossPrefab = new GameObject("BossPrefab");
        bossPrefab.SetActive(false);
        Boss bossComp = bossPrefab.AddComponent<Boss>();
        bossComp.data = ScriptableObject.CreateInstance<BossData>();

        managerObject = new GameObject("EnemyManager");
        managerObject.SetActive(false);
        enemyManager = managerObject.AddComponent<EnemyManager>();

        FieldInfo roomField = typeof(EnemyManager).GetField("room", BindingFlags.NonPublic | BindingFlags.Instance);
        roomField.SetValue(enemyManager, room);

        FieldInfo prefabsField = typeof(EnemyManager).GetField("enemyPrefabs", BindingFlags.NonPublic | BindingFlags.Instance);
        prefabsField.SetValue(enemyManager, new GameObject[] { enemyPrefab });

        FieldInfo delayField = typeof(EnemyManager).GetField("initialSpawnDelay", BindingFlags.NonPublic | BindingFlags.Instance);
        delayField.SetValue(enemyManager, 0.05f);

        roomObject.SetActive(true);
        managerObject.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(managerObject);
        UnityEngine.Object.DestroyImmediate(roomObject);
        UnityEngine.Object.DestroyImmediate(enemyPrefab);
        UnityEngine.Object.DestroyImmediate(bossPrefab);
        UnityEngine.Object.DestroyImmediate(playerObject);

        foreach (var enemy in UnityEngine.Object.FindObjectsByType<EnemyHealth>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            UnityEngine.Object.DestroyImmediate(enemy.gameObject);
        }

        foreach (var boss in UnityEngine.Object.FindObjectsByType<Boss>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            UnityEngine.Object.DestroyImmediate(boss.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator SpawnEnemiesOnEnterPreventsMultipleCalls()
    {
        enemyManager.SpawnEnemiesOnEnter();
        enemyManager.SpawnEnemiesOnEnter();

        FieldInfo spawnedField = typeof(EnemyManager).GetField("hasSpawned", BindingFlags.NonPublic | BindingFlags.Instance);
        bool hasSpawned = (bool)spawnedField.GetValue(enemyManager);

        Assert.IsTrue(hasSpawned);

        yield return null;
    }

    [UnityTest]
    public IEnumerator EmptyRoomClearsImmediatelyOnSpawn()
    {
        room.EnemySpawnPoints = new Transform[0];
        room.IsBossRoom = false;
        room.IsCleared = false; 

        enemyManager.SpawnEnemiesOnEnter();

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(room.IsCleared);

        yield return null;
    }

    [UnityTest]
    public IEnumerator NormalRoomSpawnsEnemiesAndClearsWhenAllDie()
    {
        GameObject spawn1 = new GameObject("Spawn1");
        GameObject spawn2 = new GameObject("Spawn2");
        room.EnemySpawnPoints = new Transform[] { spawn1.transform, spawn2.transform };
        room.IsBossRoom = false;
        room.IsCleared = false;

        enemyManager.SpawnEnemiesOnEnter();

        yield return new WaitForSeconds(0.1f);

        FieldInfo listField = typeof(EnemyManager).GetField("spawnedEnemies", BindingFlags.NonPublic | BindingFlags.Instance);
        List<EnemyHealth> spawnedEnemies = (List<EnemyHealth>)listField.GetValue(enemyManager);

        Assert.AreEqual(2, spawnedEnemies.Count);
        Assert.IsFalse(room.IsCleared);

        FieldInfo eventField = typeof(EnemyHealth).GetField("OnEnemyDied", BindingFlags.Instance | BindingFlags.NonPublic);
        
        MulticastDelegate del1 = (MulticastDelegate)eventField.GetValue(spawnedEnemies[0]);
        del1.DynamicInvoke(spawnedEnemies[0]);

        Assert.IsFalse(room.IsCleared);

        MulticastDelegate del2 = (MulticastDelegate)eventField.GetValue(spawnedEnemies[0]);
        del2.DynamicInvoke(spawnedEnemies[0]);

        Assert.IsTrue(room.IsCleared);

        UnityEngine.Object.DestroyImmediate(spawn1);
        UnityEngine.Object.DestroyImmediate(spawn2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator BossRoomSpawnsBossAndClearsOnlyWhenAllDie()
    {
        GameObject enemySpawn = new GameObject("EnemySpawn");
        GameObject bossSpawn = new GameObject("BossSpawn");

        room.EnemySpawnPoints = new Transform[] { enemySpawn.transform };
        room.IsBossRoom = true;
        room.BossSpawnPoint = bossSpawn.transform;
        room.BossPrefab = bossPrefab;
        room.IsCleared = false; 

        enemyManager.SpawnEnemiesOnEnter();

        yield return new WaitForSeconds(0.1f);

        FieldInfo listField = typeof(EnemyManager).GetField("spawnedEnemies", BindingFlags.NonPublic | BindingFlags.Instance);
        List<EnemyHealth> spawnedEnemies = (List<EnemyHealth>)listField.GetValue(enemyManager);

        FieldInfo bossAliveField = typeof(EnemyManager).GetField("bossAlive", BindingFlags.NonPublic | BindingFlags.Instance);
        bool isBossAlive = (bool)bossAliveField.GetValue(enemyManager);

        Assert.AreEqual(1, spawnedEnemies.Count);
        Assert.IsTrue(isBossAlive);
        Assert.IsFalse(room.IsCleared);

        Boss spawnedBoss = UnityEngine.Object.FindFirstObjectByType<Boss>(FindObjectsInactive.Include);
        
        FieldInfo bossEventField = typeof(Boss).GetField("OnBossDied", BindingFlags.Instance | BindingFlags.NonPublic);
        MulticastDelegate bossDel = (MulticastDelegate)bossEventField.GetValue(spawnedBoss);
        bossDel.DynamicInvoke();

        Assert.IsFalse(room.IsCleared);

        FieldInfo enemyEventField = typeof(EnemyHealth).GetField("OnEnemyDied", BindingFlags.Instance | BindingFlags.NonPublic);
        MulticastDelegate enemyDel = (MulticastDelegate)enemyEventField.GetValue(spawnedEnemies[0]);
        enemyDel.DynamicInvoke(spawnedEnemies[0]);

        Assert.IsTrue(room.IsCleared);

        UnityEngine.Object.DestroyImmediate(enemySpawn);
        UnityEngine.Object.DestroyImmediate(bossSpawn);

        yield return null;
    }
}
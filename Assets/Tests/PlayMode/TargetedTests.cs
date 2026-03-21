using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class TargetedTests
{
    private GameObject playerObj;
    private GameObject enemyObj;
    private GameObject roomObj;
    private GameObject dummyEnemyPrefab;

    [TearDown]
    public void Teardown()
    {
        if (playerObj != null) UnityEngine.Object.DestroyImmediate(playerObj);
        if (enemyObj != null) UnityEngine.Object.DestroyImmediate(enemyObj);
        if (roomObj != null) UnityEngine.Object.DestroyImmediate(roomObj);
        if (dummyEnemyPrefab != null) UnityEngine.Object.DestroyImmediate(dummyEnemyPrefab);

        foreach (var enemy in UnityEngine.Object.FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            UnityEngine.Object.DestroyImmediate(enemy.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator EnemyDamageDealsDamageToPlayerOnTriggerStay()
    {
        Time.timeScale = 1f;

        playerObj = new GameObject("Player");
        playerObj.SetActive(false);
        playerObj.tag = "Player";
        playerObj.AddComponent<BoxCollider2D>();
        playerObj.AddComponent<Rigidbody2D>();
        playerObj.AddComponent<Animator>();
        
        PlayerHealth health = playerObj.AddComponent<PlayerHealth>();
        PlayerStats stats = ScriptableObject.CreateInstance<PlayerStats>();
        stats.startHealth = 10f;
        stats.maxHealth = 10f;
        
        FieldInfo statsField = typeof(PlayerHealth).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(health, stats);
        
        playerObj.SetActive(true);

        enemyObj = new GameObject("Enemy");
        enemyObj.SetActive(false);
        enemyObj.AddComponent<Rigidbody2D>();
        
        Enemy enemy = enemyObj.AddComponent<Enemy>();
        enemy.data = ScriptableObject.CreateInstance<EnemyData>();
        enemy.data.damage = 2f;
        enemy.enabled = false; 
        
        EnemyDamage enemyDamage = enemyObj.AddComponent<EnemyDamage>();
        enemyObj.SetActive(true);

        yield return null;

        MethodInfo triggerMethod = typeof(EnemyDamage).GetMethod("OnTriggerStay2D", BindingFlags.NonPublic | BindingFlags.Instance);
        triggerMethod.Invoke(enemyDamage, new object[] { playerObj.GetComponent<BoxCollider2D>() });

        Assert.AreEqual(8f, health.CurrentHealth);
    }

    [UnityTest]
    public IEnumerator RoomTriggerLocksDoorsAndSpawnsEnemies()
    {
        roomObj = new GameObject("Room");
        Rooms room = roomObj.AddComponent<Rooms>();
        room.IsStarter = false;
        room.EnemySpawnPoints = new Transform[0];

        room.LeftDoor = new GameObject("LeftDoor").AddComponent<Door>();
        room.LeftDoor.ClosedDoor = new GameObject("Closed");
        room.LeftDoor.OpenDoor = new GameObject("Open");
        room.LeftDoor.Unlock();

        EnemyManager manager = roomObj.AddComponent<EnemyManager>();
        
        FieldInfo roomField = typeof(EnemyManager).GetField("room", BindingFlags.NonPublic | BindingFlags.Instance);
        roomField.SetValue(manager, room);

        FieldInfo delayField = typeof(EnemyManager).GetField("initialSpawnDelay", BindingFlags.NonPublic | BindingFlags.Instance);
        delayField.SetValue(manager, 0f);

        dummyEnemyPrefab = new GameObject("DummyEnemy");
        dummyEnemyPrefab.SetActive(false);
        dummyEnemyPrefab.AddComponent<EnemyHealth>();
        
        FieldInfo prefabsField = typeof(EnemyManager).GetField("enemyPrefabs", BindingFlags.NonPublic | BindingFlags.Instance);
        prefabsField.SetValue(manager, new GameObject[] { dummyEnemyPrefab });

        GameObject triggerObj = new GameObject("Trigger");
        triggerObj.transform.SetParent(roomObj.transform);
        RoomTrigger roomTrigger = triggerObj.AddComponent<RoomTrigger>();

        playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        BoxCollider2D playerCol = playerObj.AddComponent<BoxCollider2D>();

        yield return null;

        MethodInfo triggerMethod = typeof(RoomTrigger).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
        triggerMethod.Invoke(roomTrigger, new object[] { playerCol });

        Assert.IsFalse(room.LeftDoor.OpenDoor.activeSelf);

        FieldInfo spawnedField = typeof(EnemyManager).GetField("hasSpawned", BindingFlags.NonPublic | BindingFlags.Instance);
        bool hasSpawned = (bool)spawnedField.GetValue(manager);
        
        Assert.IsTrue(hasSpawned);

        yield return null;
    }
}
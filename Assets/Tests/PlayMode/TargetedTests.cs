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
}
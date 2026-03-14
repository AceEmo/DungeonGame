using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [Header("Room & Enemy Settings")]
    [SerializeField] private Rooms room;
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float initialSpawnDelay = 0.5f;

    private List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();
    private bool hasSpawned = false;

    private bool bossAlive = false;

    public void SpawnEnemiesOnEnter()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        StartCoroutine(SpawnAllEnemies());
    }

    private IEnumerator SpawnAllEnemies()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        if (room.IsBossRoom && room.BossSpawnPoint != null && room.BossPrefab != null)
        {
            GameObject bossInstance = Instantiate(room.BossPrefab, room.BossSpawnPoint.position, Quaternion.identity);

            Boss boss = bossInstance.GetComponent<Boss>();
            if (boss != null)
            {
                bossAlive = true;
                boss.OnBossDied += OnBossDied;
            }
        }

        foreach (Transform spawnPoint in room.EnemySpawnPoints)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, transform);

            if (enemyInstance.TryGetComponent(out EnemyHealth enemyHealth))
            {
                spawnedEnemies.Add(enemyHealth);
                enemyHealth.OnEnemyDied += OnEnemyDied;
            }
        }

        CheckRoomClear();
    }

    private void OnEnemyDied(EnemyHealth enemy)
    {
        enemy.OnEnemyDied -= OnEnemyDied;
        spawnedEnemies.Remove(enemy);

        CheckRoomClear();
    }

    private void OnBossDied()
    {
        bossAlive = false;
        CheckRoomClear();
    }

    private void CheckRoomClear()
    {
        if (room.IsBossRoom)
        {
            if (!bossAlive && spawnedEnemies.Count == 0)
            {
                room.OnRoomCleared();
            }
        }
        else
        {
            if (spawnedEnemies.Count == 0)
            {
                room.OnRoomCleared();
            }
        }
    }
}
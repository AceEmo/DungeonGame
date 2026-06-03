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

    private readonly List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();
    private bool hasSpawned;
    private bool bossAlive;

    private BossRoomController bossRoomController;

    private void Awake()
    {
        bossRoomController = GetComponentInParent<BossRoomController>();
        if (bossRoomController == null)
        {
            bossRoomController = GetComponent<BossRoomController>();
        }
    }

    public void SpawnEnemiesOnEnter()
    {
        if (hasSpawned)
        {
            return;
        }

        hasSpawned = true;
        StartCoroutine(SpawnAllEnemies());
    }

    private IEnumerator SpawnAllEnemies()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        TrySpawnBoss();
        SpawnRegularEnemies();
        CheckRoomClear();
    }

    private void TrySpawnBoss()
    {
        if (!room.IsBossRoom || room.BossSpawnPoint == null || room.BossPrefab == null)
        {
            return;
        }

        GameObject bossInstance = Instantiate(room.BossPrefab, room.BossSpawnPoint.position, Quaternion.identity);
        if (!bossInstance.TryGetComponent(out Boss boss))
        {
            return;
        }

        bossAlive = true;
        boss.OnBossDied += OnBossDied;
        bossRoomController?.InitializeBoss(boss);
    }

    private void SpawnRegularEnemies()
    {
        if (room.EnemySpawnPoints == null || enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            return;
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
        if (spawnedEnemies.Count > 0 || bossAlive)
        {
            return;
        }

        room?.OnRoomCleared();
    }
}
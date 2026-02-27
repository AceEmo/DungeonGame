using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public Rooms Room;
    public GameObject[] EnemyPrefabs;

    private List<EnemyHealth> Enemies = new List<EnemyHealth>();

    private void Start()
    {
        SpawnEnemies();

        foreach (var enemy in Enemies)
            enemy.OnEnemyDied += HandleEnemyDeath;
    }

    private void SpawnEnemies()
    {
        foreach (var point in Room.EnemySpawnPoints)
        {
            int rand = Random.Range(0, EnemyPrefabs.Length);
            GameObject e = Instantiate(EnemyPrefabs[rand], point.position, Quaternion.identity, transform);
            EnemyHealth eh = e.GetComponent<EnemyHealth>();
            if (eh != null) Enemies.Add(eh);
        }
    }

    private void HandleEnemyDeath(EnemyHealth enemy)
    {
        Enemies.Remove(enemy);

        if (Enemies.Count == 0)
        {
            Room.OnRoomCleared();
        }
    }
}
using UnityEngine;

public static class RewardSpawner
{
    public static GameObject PickRandomReward(GameObject healthPrefab, GameObject gear1Prefab, GameObject gear2Prefab)
    {
        int rewardIndex = Random.Range(0, 3);

        return rewardIndex switch
        {
            0 => healthPrefab,
            1 => gear1Prefab,
            2 => gear2Prefab,
            _ => null
        };
    }

    public static void SpawnRandomReward(Vector3 position, GameObject healthPrefab, GameObject gear1Prefab, GameObject gear2Prefab)
    {
        GameObject prefab = PickRandomReward(healthPrefab, gear1Prefab, gear2Prefab);
        if (prefab != null)
        {
            UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);
        }
    }
}

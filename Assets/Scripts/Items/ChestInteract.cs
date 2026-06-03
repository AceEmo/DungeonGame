using UnityEngine;

public class ChestInteract : MonoBehaviour, IInteractable
{
    public GameObject OpenChestPrefab;

    public GameObject HealthPrefab;
    public GameObject Gear1Prefab;
    public GameObject Gear2Prefab;

    private bool opened;

    public string GetHintText()
    {
        return "[E] Open";
    }

    public void Interact()
    {
        if (opened)
        {
            return;
        }

        opened = true;
        SpawnOpenChestVisual();
        SpawnLoot();
        Destroy(gameObject);
    }

    private void SpawnOpenChestVisual()
    {
        if (OpenChestPrefab != null)
        {
            Instantiate(OpenChestPrefab, transform.position, Quaternion.identity);
        }
    }

    private void SpawnLoot()
    {
        GameObject prefab = RewardSpawner.PickRandomReward(HealthPrefab, Gear1Prefab, Gear2Prefab);
        if (prefab == null)
        {
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}

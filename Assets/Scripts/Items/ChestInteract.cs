using UnityEngine;

public class ChestInteract : MonoBehaviour, IInteractable
{
    public GameObject OpenChestPrefab;

    public GameObject HealthPrefab;
    public GameObject Gear1Prefab;
    public GameObject Gear2Prefab;

    private bool opened = false;

    public string GetHintText()
    {
        return "[E] Open";
    }

    public void Interact()
    {
        if (opened) return;
        opened = true;

        if (OpenChestPrefab != null)
            Instantiate(OpenChestPrefab, transform.position, Quaternion.identity);

        int reward = Random.Range(0, 3);

        GameObject prefab = null;

        if (reward == 0) prefab = HealthPrefab;
        if (reward == 1) prefab = Gear1Prefab;
        if (reward == 2) prefab = Gear2Prefab;

        if (prefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
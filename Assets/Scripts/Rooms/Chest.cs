using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject OpenChestPrefab;

    public GameObject HealthPrefab;
    public GameObject Gear1Prefab;
    public GameObject Gear2Prefab;

    private bool playerNear = false;
    private bool opened = false;

    private void Update()
    {
        if (!playerNear) return;
        if (opened) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        opened = true;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}
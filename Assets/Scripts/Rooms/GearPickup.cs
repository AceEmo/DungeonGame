using UnityEngine;

public class GearPickup : MonoBehaviour
{
    public int scrapAmount = 1;

    private bool playerNear = false;
    private PlayerHealth playerHealth;

    private void Update()
    {
        if (!playerNear) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        if (playerHealth == null) return;

        GameManager.Instance.AddScrap(scrapAmount);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNear = true;
        playerHealth = other.GetComponent<PlayerHealth>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNear = false;
    }
}
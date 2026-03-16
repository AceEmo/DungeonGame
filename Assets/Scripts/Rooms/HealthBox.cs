using UnityEngine;

public class HealthBox : MonoBehaviour
{
    private bool playerNear = false;
    private PlayerHealth playerHealth;

    private void Update()
    {
        if (!playerNear) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryUse();
        }
    }

    private void TryUse()
    {
        if (playerHealth == null) return;

        if (playerHealth.CurrentHealth >= playerHealth.MaxHealth)
            return;

        float healAmount = Random.value < 0.5f ? 2f : 3f;

        playerHealth.Heal(healAmount);

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
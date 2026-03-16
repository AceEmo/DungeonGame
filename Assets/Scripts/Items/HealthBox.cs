using UnityEngine;

public class HealthBoxInteract : MonoBehaviour, IInteractable
{
    public string GetHintText()
    {
        return "[E] Heal";
    }

    public void Interact()
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (!player) return;

        if (player.CurrentHealth >= player.MaxHealth) return;

        float healAmount = Random.value < 0.5f ? 2f : 3f;
        float missingHealth = player.MaxHealth - player.CurrentHealth;
        player.Heal(Mathf.Min(healAmount, missingHealth));

        Destroy(gameObject);
    }
}
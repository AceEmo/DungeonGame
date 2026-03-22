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
        
        if (player == null || player.CurrentHealth >= player.MaxHealth)
        {
            return;
        }

        float healAmount = Random.value < 0.5f ? 2f : 3f;
        player.Heal(healAmount);

        Destroy(gameObject);
    }
}
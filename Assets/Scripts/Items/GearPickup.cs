using UnityEngine;

public class GearInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayerStats playerStats;
    public int scrapAmount = 1;

    public string GetHintText()
    {
        return "[E] Collect";
    }

    public void Interact()
    {
        if (playerStats != null)
            playerStats.AddScrap(scrapAmount);

        Destroy(gameObject);
    }
}
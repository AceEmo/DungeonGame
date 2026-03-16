using UnityEngine;

public class GearInteract : MonoBehaviour, IInteractable
{
    public int scrapAmount = 1;

    public string GetHintText()
    {
        return "[E] Collect";
    }

    public void Interact()
    {
        GameManager.Instance.AddScrap(scrapAmount);
        Destroy(gameObject);
    }
}
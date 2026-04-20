using UnityEngine;

public class UpgradeMachineInteract : MonoBehaviour, IInteractable
{
    public string GetHintText()
    {
        return "[E] Upgrade Station";
    }

    public void Interact()
    {
        GameManager.Instance.OpenUpgrade();
    }
}
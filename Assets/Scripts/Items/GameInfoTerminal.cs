using UnityEngine;

public class GameInfoTerminal : MonoBehaviour, IInteractable
{
    public string GetHintText()
    {
        return "[E] Examine Terminal";
    }

    public void Interact()
    {
        GameManager.Instance.OpenTerminal();
    }
}
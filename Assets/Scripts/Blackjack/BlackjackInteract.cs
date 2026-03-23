using UnityEngine;
using System.Collections;

public class BlackjackInteract : MonoBehaviour, IInteractable
{
    public Transform itemSpawnPoint;

    private bool canInteract = true;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterBlackjackTable(this);
        }
    }

    public string GetHintText()
    {
        return "[E] Play";
    }

    public void Interact()
    {
        if (!canInteract) return;
        if (GameManager.Instance == null) return;

        canInteract = false;
        StartCoroutine(InteractionCooldown());

        GameManager.Instance.OpenBlackjack();
    }

    IEnumerator InteractionCooldown()
    {
        yield return new WaitForSeconds(1f);
        canInteract = true;
    }
}
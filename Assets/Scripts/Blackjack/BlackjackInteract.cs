using UnityEngine;
using System.Collections;

public class BlackjackInteract : MonoBehaviour, IInteractable
{
    public GameObject blackjackCanvas;
    private bool canInteract = true;

    public string GetHintText()
    {
        return "[E] Play";
    }

    public void Interact()
{
    if (!canInteract) return;

    canInteract = false;
    StartCoroutine(InteractionCooldown());

    if (blackjackCanvas == null) return;

    blackjackCanvas.SetActive(true);
    Time.timeScale = 0f;

    BlackjackGame game = blackjackCanvas.GetComponent<BlackjackGame>();
    if (game != null)
        game.StartBlackjack();
}

    IEnumerator InteractionCooldown()
    {
        yield return new WaitForSeconds(1f);
        canInteract = true;
    }
}
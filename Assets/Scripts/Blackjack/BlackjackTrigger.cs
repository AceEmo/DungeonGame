using UnityEngine;

public class BlackjackTrigger : MonoBehaviour
{
    public GameObject blackjackCanvas;
    private bool playerInside;
    private bool alreadyPlayed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInside = false;
    }

    private void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
            OpenBlackjack();
    }

    void OpenBlackjack()
    {
        if (alreadyPlayed) return;

        blackjackCanvas.SetActive(true);
        Time.timeScale = 0f;

        blackjackCanvas.GetComponent<BlackjackGame>().StartBlackjack();
        alreadyPlayed = true;
    }
}
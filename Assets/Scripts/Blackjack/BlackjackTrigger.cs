using UnityEngine;

public class BlackjackTrigger : MonoBehaviour
{
    public GameObject blackjackCanvas;
    private bool playerInside;

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
        blackjackCanvas.SetActive(true);
        Time.timeScale = 0f;

        blackjackCanvas.GetComponent<BlackjackGame>().StartBlackjack();
    }
}

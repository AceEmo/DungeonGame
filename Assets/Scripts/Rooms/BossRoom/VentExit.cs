using UnityEngine;

public class VentExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandleVentInteraction();
        }
    }

    private void HandleVentInteraction()
    {
        int currentLevel = GameManager.Instance.currentLevel;
        int maxLevels = GameManager.Instance.Settings.MaxLevels;

        if (currentLevel >= maxLevels)
        {
            GameManager.Instance.HandleGameWin();
        }
        else
        {
            GameManager.Instance.LoadNextLevel();
        }
    }
}
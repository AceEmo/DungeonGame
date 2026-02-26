using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        playerHealth.OnPlayerDied += HandleGameOver;
    }

    public void RestartGame()
    {
        if (playerHealth != null)
            playerHealth.ResetHealth();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void HandleGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
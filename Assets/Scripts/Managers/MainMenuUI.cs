using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame(); 
        }
    }

    public void QuitGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ExitGame();
        }
    }
}
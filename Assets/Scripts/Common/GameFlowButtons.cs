using UnityEngine;
using UnityEngine.UI;

public static class GameFlowButtons
{
    public static void Bind(Button restartButton, Button exitButton)
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

    public static void Unbind(Button restartButton, Button exitButton)
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(ExitGame);
    }

    private static void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    private static void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }
}

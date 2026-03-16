using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }
    }
    private void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }

    private void OnExitClicked()
    {
        GameManager.Instance.ExitGame();
    }

    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitClicked);
        }
    }
}
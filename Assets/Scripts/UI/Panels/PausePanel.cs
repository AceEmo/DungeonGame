using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        GameFlowButtons.Bind(restartButton, exitButton);
    }

    private void OnDestroy()
    {
        GameFlowButtons.Unbind(restartButton, exitButton);
    }
}

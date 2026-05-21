using UnityEngine;

public class PanelController
{
    public GameObject PausePanel { get; set; }
    public GameObject GameOverPanel { get; set; }
    public GameObject BlackjackCanvas { get; set; }
    public GameObject TerminalPanel { get; set; }
    public GameObject UpgradePanel { get; set; }

    public void FindSceneReferences()
    {
        PausePanel = Object.FindFirstObjectByType<PausePanel>(FindObjectsInactive.Include)?.gameObject;
        GameOverPanel = Object.FindFirstObjectByType<GameOverPanel>(FindObjectsInactive.Include)?.gameObject;
        TerminalPanel = Object.FindFirstObjectByType<TerminalPanel>(FindObjectsInactive.Include)?.gameObject;
        UpgradePanel = Object.FindFirstObjectByType<UpdatePanel>(FindObjectsInactive.Include)?.gameObject;
        BlackjackCanvas = Object.FindFirstObjectByType<BlackjackGame>(FindObjectsInactive.Include)?.gameObject;
    }

    public void UpdateUIStates(GameState state)
    {
        TogglePanel(PausePanel, state == GameState.Paused);
        TogglePanel(GameOverPanel, state == GameState.GameOver);
        TogglePanel(BlackjackCanvas, state == GameState.Blackjack);
        TogglePanel(TerminalPanel, state == GameState.Terminal);
        TogglePanel(UpgradePanel, state == GameState.Upgrade);
    }

    private void TogglePanel(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }
}
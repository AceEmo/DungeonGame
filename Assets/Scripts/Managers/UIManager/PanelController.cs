using UnityEngine;

public class PanelController
{
    public GameObject PausePanel { get; set; }
    public GameObject GameOverPanel { get; set; }
    public GameObject BlackjackCanvas { get; set; }
    public GameObject TerminalPanel { get; set; }
    public GameObject UpgradePanel { get; set; }
    public GameObject WinScreenPanel { get; set; }

    public void FindSceneReferences()
    {
        PausePanel = Object.FindFirstObjectByType<PausePanel>(FindObjectsInactive.Include)?.gameObject;
        GameOverPanel = Object.FindFirstObjectByType<GameOverPanel>(FindObjectsInactive.Include)?.gameObject;
        TerminalPanel = Object.FindFirstObjectByType<TerminalPanel>(FindObjectsInactive.Include)?.gameObject;
        BlackjackCanvas = Object.FindFirstObjectByType<BlackjackGame>(FindObjectsInactive.Include)?.gameObject;
        WinScreenPanel = Object.FindFirstObjectByType<WinScreenPanel>(FindObjectsInactive.Include)?.gameObject;
        UpgradePanel = Object.FindFirstObjectByType<UpgradePanel>(FindObjectsInactive.Include)?.gameObject;
    }

    public void UpdateUIStates(GameState state)
    {
        TogglePanel(PausePanel, state == GameState.Paused);
        TogglePanel(GameOverPanel, state == GameState.GameOver);
        TogglePanel(BlackjackCanvas, state == GameState.Blackjack);
        TogglePanel(TerminalPanel, state == GameState.Terminal);
        TogglePanel(UpgradePanel, state == GameState.Upgrade);
        TogglePanel(WinScreenPanel, state == GameState.WinScreen);
    }

    private void TogglePanel(GameObject panel, bool isActive)
    {
        if (panel == null) return;

        if (panel.activeSelf != isActive)
        {
            panel.SetActive(isActive);
        }
    }
}

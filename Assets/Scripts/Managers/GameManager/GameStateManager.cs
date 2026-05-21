using UnityEngine;

public class GameStateManager
{
    private GameState currentState;
    private readonly PanelController panelController;

    public GameState CurrentState => currentState;

    public GameStateManager(PanelController panelController)
    {
        this.panelController = panelController;
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
        
        UpdateTimeScale();
        UpdateCursorState();
        panelController.UpdateUIStates(currentState);
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = (currentState == GameState.Gameplay) ? 1f : 0f;
    }

    private void UpdateCursorState()
    {
        bool isUIState = currentState != GameState.Gameplay;
        Cursor.lockState = isUIState ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isUIState;
    }
}
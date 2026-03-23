using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Gameplay,
    Paused,
    GameOver,
    Blackjack,
    Terminal
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerStats playerStats;

    private PlayerHealth playerHealth;
    private GameObject gameOverPanel;
    private GameObject pausePanel;
    private GameObject blackjackCanvas;
    private GameObject terminalPanel;
    private GameState currentState;

    private BlackjackInteract currentBlackjackTable;

    public void OpenTerminal() => SetGameState(GameState.Terminal);
    public void CloseTerminal() => SetGameState(GameState.Gameplay);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeSceneData();
    }

    private void InitializeSceneData()
    {
        RemoveDuplicateEventSystems();
        FindAndAssignSceneReferences();
        SetGameState(GameState.Gameplay);

        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
            playerHealth.OnPlayerDied += HandleGameOver;
        }

        currentBlackjackTable = null;
    }

    private void RemoveDuplicateEventSystems()
    {
        var systems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
        if (systems.Length > 1)
        {
            for (int i = 1; i < systems.Length; i++)
            {
                Destroy(systems[i].gameObject);
            }
        }
    }

    private void FindAndAssignSceneReferences()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        PausePanel pausePanelRef = FindFirstObjectByType<PausePanel>(FindObjectsInactive.Include);
        if (pausePanelRef != null) pausePanel = pausePanelRef.gameObject;

        GameOverPanel gameOverPanelRef = FindFirstObjectByType<GameOverPanel>(FindObjectsInactive.Include);
        if (gameOverPanelRef != null) gameOverPanel = gameOverPanelRef.gameObject;

        TerminalPanel terminalPanelRef = FindFirstObjectByType<TerminalPanel>(FindObjectsInactive.Include);
        if (terminalPanelRef != null) terminalPanel = terminalPanelRef.gameObject;

        BlackjackGame blackjackGameRef = FindFirstObjectByType<BlackjackGame>(FindObjectsInactive.Include);
        if (blackjackGameRef != null) blackjackCanvas = blackjackGameRef.gameObject;
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;
        
        UpdateTimeScale();
        UpdateCursorState();
        UpdateUIStates();
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = (currentState == GameState.Gameplay) ? 1f : 0f;
    }

    private void UpdateCursorState()
    {
        bool isGameplay = currentState == GameState.Gameplay;

        if (isGameplay)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void UpdateUIStates()
    {
        TogglePanel(pausePanel, currentState == GameState.Paused);
        TogglePanel(gameOverPanel, currentState == GameState.GameOver);
        TogglePanel(blackjackCanvas, currentState == GameState.Blackjack);
        TogglePanel(terminalPanel, currentState == GameState.Terminal);
    }

    private void TogglePanel(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (currentState == GameState.Blackjack)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Gameplay)
                SetGameState(GameState.Paused);
            else if (currentState == GameState.Paused)
                SetGameState(GameState.Gameplay);
        }
    }

    private void HandleGameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void RegisterBlackjackTable(BlackjackInteract table)
    {
        currentBlackjackTable = table;
    }

    public void OpenBlackjack()
    {
        if (currentState == GameState.GameOver)
            return;

        if (blackjackCanvas == null || currentBlackjackTable == null)
            return;

        SetGameState(GameState.Blackjack);

        var game = blackjackCanvas.GetComponent<BlackjackGame>();
        if (game != null)
        {
            game.SetItemSpawnPoint(currentBlackjackTable.itemSpawnPoint);
            game.StartBlackjack();
        }
    }

    public void CloseBlackjack()
    {
        if (currentState == GameState.Blackjack)
        {
            SetGameState(GameState.Gameplay);
        }
    }

    public void RestartGame()
    {
        if (playerStats != null)
        {
            playerStats.ResetAll();
        }

        SceneManager.LoadScene("HubRoom");
    }

    public bool IsGameplayActive()
    {
        return currentState == GameState.Gameplay;
    }

    public void RegisterBlackjackCanvas(GameObject canvas)
    {
        blackjackCanvas = canvas;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
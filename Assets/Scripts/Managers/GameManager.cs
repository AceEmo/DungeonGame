using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Gameplay,
    Paused,
    GameOver,
    Blackjack,
    Terminal,
    Upgrade
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
    private GameObject upgradePanel;
    private GameState currentState;

    private BlackjackInteract currentBlackjackTable;

    public void OpenTerminal() => SetGameState(GameState.Terminal);
    public void CloseTerminal() => SetGameState(GameState.Gameplay);
    public void OpenUpgrade() => SetGameState(GameState.Upgrade);
    public void CloseUpgrade() => SetGameState(GameState.Gameplay);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            EnsureEventSystemExists();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnsureEventSystemExists()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }
    }

    public int currentLevel = 0;
    public int maxLevels = 5;

    public void LoadNextLevel()
    {
        currentLevel++;

        if (currentLevel > maxLevels)
        {
            SceneManager.LoadScene("WinScreen");
            return;
        }

        SceneManager.LoadScene("Level" + currentLevel);
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
        EnsureEventSystemExists();
        if (scene.name == "MainMenu")
        {
            SetGameState(GameState.MainMenu);
        }
        else
        {
            InitializeSceneData();
        }
    }

    private void InitializeSceneData()
    {
        FindAndAssignSceneReferences();
        SetGameState(GameState.Gameplay);

        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
            playerHealth.OnPlayerDied += HandleGameOver;
        }

        currentBlackjackTable = null;
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

        UpdatePanel upgradePanelRef = FindFirstObjectByType<UpdatePanel>(FindObjectsInactive.Include);
        if (upgradePanelRef != null) upgradePanel = upgradePanelRef.gameObject;

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
        bool isUIState = currentState != GameState.Gameplay;
        Cursor.lockState = isUIState ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isUIState;

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
        TogglePanel(upgradePanel, currentState == GameState.Upgrade);
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
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

        PausePanel pause = FindFirstObjectByType<PausePanel>(FindObjectsInactive.Include);
        if (pause != null) pausePanel = pause.gameObject;

        GameOverPanel gameOver = FindFirstObjectByType<GameOverPanel>(FindObjectsInactive.Include);
        if (gameOver != null) gameOverPanel = gameOver.gameObject;

        TerminalPanel terminal = FindFirstObjectByType<TerminalPanel>(FindObjectsInactive.Include);
        if (terminal != null) terminalPanel = terminal.gameObject;
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;
        UpdateUIStates();

        switch (currentState)
        {
            case GameState.Gameplay:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
            case GameState.Blackjack:
                Time.timeScale = 1f;
                break;
            case GameState.Terminal:
                Time.timeScale = 0f;
                break;
        }
    }

    private void UpdateUIStates()
    {
        if (pausePanel != null) pausePanel.SetActive(currentState == GameState.Paused);
        if (gameOverPanel != null) gameOverPanel.SetActive(currentState == GameState.GameOver);
        if (blackjackCanvas != null) blackjackCanvas.SetActive(currentState == GameState.Blackjack);
        if (terminalPanel != null) terminalPanel.SetActive(currentState == GameState.Terminal);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Gameplay)
                SetGameState(GameState.Paused);
            else if (currentState == GameState.Paused || currentState == GameState.Blackjack)
                SetGameState(GameState.Gameplay);
        }
    }

    private void HandleGameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void OpenBlackjack()
    {
        if (currentState != GameState.GameOver)
            SetGameState(GameState.Blackjack);
    }

    public void CloseBlackjack()
    {
        if (currentState == GameState.Blackjack)
            SetGameState(GameState.Gameplay);
    }

    public void RegisterBlackjackCanvas(GameObject canvas)
    {
        blackjackCanvas = canvas;
    }

    public void RestartGame()
    {
        if (playerStats != null)
            playerStats.ResetAll();

        Time.timeScale = 1f;
        SceneManager.LoadScene("HubRoom");
    }

    public bool IsGameplayActive()
    {
        return currentState == GameState.Gameplay;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
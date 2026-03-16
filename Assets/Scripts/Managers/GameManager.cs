using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Gameplay,
    Paused,
    GameOver,
    Blackjack
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Scrap { get; private set; }

    private PlayerHealth playerHealth;
    private GameObject gameOverPanel;
    private GameObject pausePanel;
    private GameObject blackjackCanvas;

    private GameState currentState;

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

    private void Update()
    {
        HandleInput();
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
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied += HandleGameOver;
        }

        PausePanel pause = FindFirstObjectByType<PausePanel>(FindObjectsInactive.Include);
        if (pause != null) pausePanel = pause.gameObject;

        GameOverPanel gameOver = FindFirstObjectByType<GameOverPanel>(FindObjectsInactive.Include);
        if (gameOver != null) gameOverPanel = gameOver.gameObject;
    }

    private void SetGameState(GameState newState)
    {
        currentState = newState;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (blackjackCanvas != null) blackjackCanvas.SetActive(false);

        if (InteractionUI.Instance != null)
        {
            InteractionUI.Instance.HideHint();
        }

        switch (currentState)
        {
            case GameState.Gameplay:
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                if (pausePanel != null) pausePanel.SetActive(true);
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                break;

            case GameState.Blackjack:
                Time.timeScale = 1f;
                if (blackjackCanvas != null) blackjackCanvas.SetActive(true);
                break;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Gameplay)
            {
                SetGameState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                SetGameState(GameState.Gameplay);
            }
            else if (currentState == GameState.Blackjack)
            {
                SetGameState(GameState.Gameplay);
            }
        }
    }

    private void HandleGameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void RegisterBlackjackCanvas(GameObject canvas)
    {
        if (canvas == null) return;
        blackjackCanvas = canvas;
    }

    public void UnregisterBlackjackCanvas(GameObject canvas)
    {
        if (canvas != null && blackjackCanvas == canvas)
        {
            blackjackCanvas = null;
        }
    }

    public void OpenBlackjack()
    {
        if (currentState != GameState.GameOver)
        {
            SetGameState(GameState.Blackjack);
        }
    }

    public void CloseBlackjack()
    {
        if (currentState == GameState.Blackjack)
        {
            SetGameState(GameState.Gameplay);
        }
    }

    public bool IsGameplayActive()
    {
        return currentState == GameState.Gameplay;
    }

    public void AddScrap(int amount)
    {
        Scrap += amount;
    }

    public void ResetRun()
    {
        Scrap = 0;
    }

    public void RestartGame()
    {
        ResetRun();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
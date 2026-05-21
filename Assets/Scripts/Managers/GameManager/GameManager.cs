using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Dependencies")]
    public PlayerStats playerStats;

    private GameStateManager stateManager;
    private PanelController panelController;
    private LevelProgressionManager levelManager;

    private PlayerHealth playerHealth;
    private BlackjackInteract currentBlackjackTable;

    public int currentLevel => levelManager.CurrentLevel;
    public GameState CurrentState => stateManager.CurrentState;

    #region Public API
    public void ResetGameProgress() => levelManager.ResetLevels();
    public void OpenTerminal() => stateManager.SetState(GameState.Terminal);
    public void CloseTerminal() => stateManager.SetState(GameState.Gameplay);
    public void OpenUpgrade() => stateManager.SetState(GameState.Upgrade);
    public void CloseUpgrade() => stateManager.SetState(GameState.Gameplay);
    public bool IsGameplayActive() => stateManager.CurrentState == GameState.Gameplay;
    public void LoadNextLevel() => levelManager.LoadNextLevel();
    public void RegisterBlackjackCanvas(GameObject canvas) => panelController.BlackjackCanvas = canvas;
    public void RegisterBlackjackTable(BlackjackInteract table) => currentBlackjackTable = table;
    public void ExitGame() => Application.Quit();
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeComponents();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeComponents()
    {
        panelController = new PanelController();
        stateManager = new GameStateManager(panelController);
        levelManager = new LevelProgressionManager();
        EnsureEventSystemExists();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (stateManager.CurrentState == GameState.Blackjack) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (stateManager.CurrentState == GameState.Gameplay)
                stateManager.SetState(GameState.Paused);
            else if (stateManager.CurrentState == GameState.Paused)
                stateManager.SetState(GameState.Gameplay);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureEventSystemExists();

        if (scene.name == "MainMenu")
        {
            stateManager.SetState(GameState.MainMenu);
            return;
        }

        InitializeSceneData(scene.name);
    }

    private void InitializeSceneData(string sceneName)
    {
        panelController.FindSceneReferences();
        stateManager.SetState(GameState.Gameplay);

        playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            HandlePlayerHealthInitialization(sceneName);
        }

        currentBlackjackTable = null;
    }

    private void HandlePlayerHealthInitialization(string sceneName)
    {
        if (sceneName == "HubRoom")
        {
            playerHealth.ResetHealth();
        }

        playerHealth.OnPlayerDied -= HandleGameOver;
        playerHealth.OnPlayerDied += HandleGameOver;
    }

    private void HandleGameOver() => stateManager.SetState(GameState.GameOver);

    public void OpenBlackjack()
    {
        if (stateManager.CurrentState == GameState.GameOver) return;
        if (panelController.BlackjackCanvas == null || currentBlackjackTable == null) return;

        stateManager.SetState(GameState.Blackjack);

        var game = panelController.BlackjackCanvas.GetComponent<BlackjackGame>();
        if (game != null)
        {
            game.SetItemSpawnPoint(currentBlackjackTable.itemSpawnPoint);
            game.StartBlackjack();
        }
    }

    public void CloseBlackjack()
    {
        if (stateManager.CurrentState == GameState.Blackjack)
        {
            stateManager.SetState(GameState.Gameplay);
        }
    }

    public void RestartGame()
    {
        if (playerStats != null) playerStats.ResetAll();
        SceneManager.LoadScene("HubRoom");
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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }
}
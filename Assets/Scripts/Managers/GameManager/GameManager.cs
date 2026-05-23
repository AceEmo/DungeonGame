using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
    public GameSettings Settings { get; private set; } = new GameSettings();

    private AudioSource audioSource;

    [Header("UI Audio")]
    [SerializeField] private AudioClip restartSound;
    [SerializeField] [Range(0f, 1f)] private float restartVolume = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        Settings.LoadFromPrefs();

        InitializeComponents();
        ApplySettingsToGame();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void InitializeComponents()
    {
        panelController = new PanelController();
        stateManager = new GameStateManager(panelController);
        levelManager = new LevelProgressionManager();

        EnsureEventSystemExists();
    }

    private void ApplySettingsToGame()
    {
        levelManager.MaxLevels = Settings.MaxLevels;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (stateManager.CurrentState == GameState.Blackjack || 
            stateManager.CurrentState == GameState.WinScreen || 
            stateManager.CurrentState == GameState.GameOver) 
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
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
            playerHealth.OnPlayerDied -= HandleGameOver;
            playerHealth.OnPlayerDied += HandleGameOver;

            if (sceneName == "HubRoom")
                playerHealth.ResetHealth();
        }

        currentBlackjackTable = null;
    }

    private void HandleGameOver()
    {
        stateManager.SetState(GameState.GameOver);
    }

    public void ResetGameProgress()
    {
        levelManager.ResetLevels();
    }

    public void OpenTerminal() => stateManager.SetState(GameState.Terminal);
    public void CloseTerminal() => stateManager.SetState(GameState.Gameplay);
    public void OpenUpgrade() => stateManager.SetState(GameState.Upgrade);
    public void CloseUpgrade() => stateManager.SetState(GameState.Gameplay);

    public bool IsGameplayActive() => stateManager.CurrentState == GameState.Gameplay;

    public void LoadNextLevel() => levelManager.LoadNextLevel();

    public void RegisterBlackjackCanvas(GameObject canvas)
    {
        panelController.BlackjackCanvas = canvas;
    }

    public void RegisterBlackjackTable(BlackjackInteract table)
    {
        currentBlackjackTable = table;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ApplySettingsFromUI()
    {
        levelManager.MaxLevels = Settings.MaxLevels;
    }

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
            stateManager.SetState(GameState.Gameplay);
    }

    public void RestartGame()
    {
        if (audioSource != null && restartSound != null)
            audioSource.PlayOneShot(restartSound, restartVolume);

        levelManager.ResetLevels();
        ApplySettingsToGame();

        if (playerStats != null)
            playerStats.ResetAll();

        SceneManager.LoadScene("HubRoom");
    }

    private void EnsureEventSystemExists()
    {
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            DontDestroyOnLoad(es);
        }
    }

    public void HandleGameWin()
    {
        stateManager.SetState(GameState.WinScreen);
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
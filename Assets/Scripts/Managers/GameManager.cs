using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;

    public int Scrap { get; private set; }

    private bool isPaused = false;
    private bool isGameOver = false;

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
            return;
        }
    }
    void FixEventSystem()
    {
        var systems = FindObjectsByType<UnityEngine.EventSystems.EventSystem>(
            FindObjectsSortMode.None
        );

        if (systems.Length > 1)
        {
            for (int i = 1; i < systems.Length; i++)
            {
                Destroy(systems[i].gameObject);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupSceneReferences();
    }

    private void Start()
    {
        SetupSceneReferences();
    }

    private void SetupSceneReferences()
    {
        FixEventSystem();

        playerHealth = FindFirstObjectByType<PlayerHealth>();

        PausePanel pause = FindFirstObjectByType<PausePanel>(FindObjectsInactive.Include);
        GameOverPanel gameOver = FindFirstObjectByType<GameOverPanel>(FindObjectsInactive.Include);

        if (pause != null)
            pausePanel = pause.gameObject;

        if (gameOver != null)
            gameOverPanel = gameOver.gameObject;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (playerHealth != null)
            playerHealth.OnPlayerDied += HandleGameOver;

        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGameOver)
                TogglePause();
        }
    }

    private void TogglePause()
    {
        if (pausePanel == null) return;

        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void HandleGameOver()
    {
        isGameOver = true;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
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
        Time.timeScale = 1f;
        ResetRun();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
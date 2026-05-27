using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinScreenPanel : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject winMainPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI levelsValueLabel;
    [SerializeField] private TextMeshProUGUI difficultyValueLabel;

    [Header("Sliders")]
    [SerializeField] private Slider levelsSlider;
    [SerializeField] private Slider difficultySlider;

    private GameSettings temporarySettings;

    private void Start()
    {
        InitializeSettings();
        ConfigureSliders();
        ApplySlidersFromSettings();
        UpdateUI();
        RegisterSliderListeners();
    }

    private void OnDestroy()
    {
        UnregisterSliderListeners();
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
    }

    public void QuitGame()
    {
        GameManager.Instance.ExitGame();
    }

    public void OpenSettings()
    {
        if (winMainPanel != null) winMainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);

        temporarySettings = new GameSettings(GameManager.Instance.Settings);

        ApplySlidersFromSettings();
        UpdateUI();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (winMainPanel != null) winMainPanel.SetActive(true);
    }

    public void SaveSettings()
    {
        GameManager.Instance.Settings.MaxLevels = temporarySettings.MaxLevels;
        GameManager.Instance.Settings.Difficulty = temporarySettings.Difficulty;

        GameManager.Instance.Settings.SaveToPrefs();
        GameManager.Instance.ApplySettingsFromUI();

        CloseSettings();
    }

    private void InitializeSettings()
    {
        temporarySettings = new GameSettings(GameManager.Instance.Settings);
    }

    private void ConfigureSliders()
    {
        if (levelsSlider != null)
        {
            levelsSlider.wholeNumbers = true;
            levelsSlider.minValue = GameSettings.MinLevelsLimit;
            levelsSlider.maxValue = GameSettings.MaxLevelsLimit;
        }

        if (difficultySlider != null)
        {
            difficultySlider.wholeNumbers = true;
            difficultySlider.minValue = 0;
            difficultySlider.maxValue = (int)GameDifficulty.Hard;
        }
    }

    private void ApplySlidersFromSettings()
    {
        if (levelsSlider != null)
            levelsSlider.SetValueWithoutNotify(temporarySettings.MaxLevels);

        if (difficultySlider != null)
            difficultySlider.SetValueWithoutNotify((int)temporarySettings.Difficulty);
    }

    private void RegisterSliderListeners()
    {
        if (levelsSlider != null)
            levelsSlider.onValueChanged.AddListener(HandleMaxLevelsChanged);

        if (difficultySlider != null)
            difficultySlider.onValueChanged.AddListener(HandleDifficultyChanged);
    }

    private void UnregisterSliderListeners()
    {
        if (levelsSlider != null)
            levelsSlider.onValueChanged.RemoveListener(HandleMaxLevelsChanged);

        if (difficultySlider != null)
            difficultySlider.onValueChanged.RemoveListener(HandleDifficultyChanged);
    }

    private void HandleMaxLevelsChanged(float value)
    {
        temporarySettings.MaxLevels = Mathf.RoundToInt(value);
        UpdateUI();
    }

    private void HandleDifficultyChanged(float value)
    {
        temporarySettings.Difficulty = (GameDifficulty)Mathf.RoundToInt(value);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (levelsValueLabel != null)
            levelsValueLabel.text = $"Levels: {temporarySettings.MaxLevels}";

        if (difficultyValueLabel != null)
            difficultyValueLabel.text = $"Difficulty: {temporarySettings.Difficulty}";
    }
}
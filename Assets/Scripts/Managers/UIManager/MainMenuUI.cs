using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
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
        temporarySettings = new GameSettings(GameManager.Instance.Settings);

        ConfigureSliders();
        ApplySlidersFromSettings();
        UpdateUI();
        
        RegisterSliderListeners();
    }

    public void StartGame()
    {
        GameManager.Instance.RestartGame();
    }

    public void QuitGame()
    {
        GameManager.Instance.ExitGame();
    }

    public void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        temporarySettings = new GameSettings(GameManager.Instance.Settings);

        ApplySlidersFromSettings();
        UpdateUI();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SaveSettings()
    {
        GameManager.Instance.Settings.MaxLevels = temporarySettings.MaxLevels;
        GameManager.Instance.Settings.Difficulty = temporarySettings.Difficulty;

        GameManager.Instance.Settings.SaveToPrefs();
        GameManager.Instance.ApplySettingsFromUI();

        CloseSettings();
    }

    private void ConfigureSliders()
    {
        levelsSlider.wholeNumbers = true;
        levelsSlider.minValue = GameSettings.MinLevelsLimit;
        levelsSlider.maxValue = GameSettings.MaxLevelsLimit;

        difficultySlider.wholeNumbers = true;
        difficultySlider.minValue = 0;
        difficultySlider.maxValue = (int)GameDifficulty.Hard;
    }

    private void ApplySlidersFromSettings()
    {
        levelsSlider.SetValueWithoutNotify(temporarySettings.MaxLevels);
        difficultySlider.SetValueWithoutNotify((int)temporarySettings.Difficulty);
    }

    private void RegisterSliderListeners()
    {
        levelsSlider.onValueChanged.AddListener(UpdateMaxLevelsFromSlider);
        difficultySlider.onValueChanged.AddListener(UpdateDifficultyFromSlider);
    }

    private void UpdateMaxLevelsFromSlider(float value)
    {
        temporarySettings.MaxLevels = Mathf.RoundToInt(value);
        UpdateUI();
    }

    private void UpdateDifficultyFromSlider(float value)
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

    private void OnDestroy()
    {
        if (levelsSlider != null)
            levelsSlider.onValueChanged.RemoveListener(UpdateMaxLevelsFromSlider);

        if (difficultySlider != null)
            difficultySlider.onValueChanged.RemoveListener(UpdateDifficultyFromSlider);
    }
}

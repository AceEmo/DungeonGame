using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettingsUIController
{
    private readonly Slider levelsSlider;
    private readonly Slider difficultySlider;
    private readonly TextMeshProUGUI levelsValueLabel;
    private readonly TextMeshProUGUI difficultyValueLabel;

    private GameSettings temporarySettings;

    public GameSettings TemporarySettings => temporarySettings;

    public GameSettingsUIController(
        Slider levelsSlider,
        Slider difficultySlider,
        TextMeshProUGUI levelsValueLabel,
        TextMeshProUGUI difficultyValueLabel)
    {
        this.levelsSlider = levelsSlider;
        this.difficultySlider = difficultySlider;
        this.levelsValueLabel = levelsValueLabel;
        this.difficultyValueLabel = difficultyValueLabel;
    }

    public void Initialize()
    {
        temporarySettings = new GameSettings(GameManager.Instance.Settings);
        ConfigureSliders();
        ApplySlidersFromSettings();
        UpdateLabels();
        RegisterListeners();
    }

    public void ReloadFromGameManager()
    {
        temporarySettings = new GameSettings(GameManager.Instance.Settings);
        ApplySlidersFromSettings();
        UpdateLabels();
    }

    public void SaveToGameManager()
    {
        GameManager.Instance.Settings.MaxLevels = temporarySettings.MaxLevels;
        GameManager.Instance.Settings.Difficulty = temporarySettings.Difficulty;
        GameManager.Instance.Settings.SaveToPrefs();
        GameManager.Instance.ApplySettingsFromUI();
    }

    public void UnregisterListeners()
    {
        if (levelsSlider != null)
            levelsSlider.onValueChanged.RemoveListener(HandleMaxLevelsChanged);

        if (difficultySlider != null)
            difficultySlider.onValueChanged.RemoveListener(HandleDifficultyChanged);
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

    private void RegisterListeners()
    {
        if (levelsSlider != null)
            levelsSlider.onValueChanged.AddListener(HandleMaxLevelsChanged);

        if (difficultySlider != null)
            difficultySlider.onValueChanged.AddListener(HandleDifficultyChanged);
    }

    private void HandleMaxLevelsChanged(float value)
    {
        temporarySettings.MaxLevels = Mathf.RoundToInt(value);
        UpdateLabels();
    }

    private void HandleDifficultyChanged(float value)
    {
        temporarySettings.Difficulty = (GameDifficulty)Mathf.RoundToInt(value);
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        if (levelsValueLabel != null)
            levelsValueLabel.text = $"Levels: {temporarySettings.MaxLevels}";

        if (difficultyValueLabel != null)
            difficultyValueLabel.text = $"Difficulty: {temporarySettings.Difficulty}";
    }
}

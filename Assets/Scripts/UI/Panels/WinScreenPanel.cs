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

    private GameSettingsUIController settingsController;

    private void Start()
    {
        settingsController = new GameSettingsUIController(
            levelsSlider, difficultySlider, levelsValueLabel, difficultyValueLabel);
        settingsController.Initialize();
    }

    private void OnDestroy()
    {
        settingsController?.UnregisterListeners();
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
        settingsController.ReloadFromGameManager();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (winMainPanel != null) winMainPanel.SetActive(true);
    }

    public void SaveSettings()
    {
        settingsController.SaveToGameManager();
        CloseSettings();
    }
}

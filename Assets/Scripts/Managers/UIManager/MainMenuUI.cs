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

    private GameSettingsUIController settingsController;

    private void Start()
    {
        settingsController = new GameSettingsUIController(
            levelsSlider, difficultySlider, levelsValueLabel, difficultyValueLabel);
        settingsController.Initialize();
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
        settingsController.ReloadFromGameManager();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SaveSettings()
    {
        settingsController.SaveToGameManager();
        CloseSettings();
    }

    private void OnDestroy()
    {
        settingsController?.UnregisterListeners();
    }
}

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class MainMenuUITests
{
    private GameObject _uiObject;
    private GameObject _mainMenuPanel;
    private GameObject _settingsPanel;
    private GameObject _gameManagerObject;
    private MainMenuUI _mainMenuUI;

    [SetUp]
    public void SetUp()
    {
        _gameManagerObject = new GameObject("GameManager");
        _gameManagerObject.AddComponent<GameManager>();

        _uiObject = new GameObject("MainMenuUI");
        _mainMenuPanel = new GameObject("MainMenuPanel");
        _settingsPanel = new GameObject("SettingsPanel");

        _mainMenuUI = _uiObject.AddComponent<MainMenuUI>();

        typeof(MainMenuUI).GetField("mainMenuPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mainMenuUI, _mainMenuPanel);
        typeof(MainMenuUI).GetField("settingsPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mainMenuUI, _settingsPanel);
        typeof(MainMenuUI).GetField("levelsSlider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mainMenuUI, new GameObject().AddComponent<Slider>());
        typeof(MainMenuUI).GetField("difficultySlider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mainMenuUI, new GameObject().AddComponent<Slider>());
        typeof(MainMenuUI).GetField("levelsValueLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mainMenuUI, new GameObject().AddComponent<TextMeshProUGUI>());
        typeof(MainMenuUI).GetField("difficultyValueLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_mainMenuUI, new GameObject().AddComponent<TextMeshProUGUI>());
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_uiObject);
        Object.Destroy(_mainMenuPanel);
        Object.Destroy(_settingsPanel);
        Object.Destroy(_gameManagerObject);
        
        typeof(GameManager).GetProperty("Instance").SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator OpenSettings_ShouldDeactivateMainMenuAndActivateSettingsPanel()
    {
        _mainMenuUI.SendMessage("Start");
        yield return null;

        _mainMenuUI.OpenSettings();

        Assert.IsFalse(_mainMenuPanel.activeSelf);
        Assert.IsTrue(_settingsPanel.activeSelf);
    }

    [UnityTest]
    public IEnumerator CloseSettings_ShouldDeactivateSettingsAndActivateMainMenuPanel()
    {
        _mainMenuUI.SendMessage("Start");
        yield return null;
        _mainMenuUI.OpenSettings();

        _mainMenuUI.CloseSettings();

        Assert.IsTrue(_mainMenuPanel.activeSelf);
        Assert.IsFalse(_settingsPanel.activeSelf);
    }
}
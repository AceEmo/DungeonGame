using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class WinScreenPanelTests
{
    private GameObject _gameManagerObject;
    private GameObject _panelObject;
    private WinScreenPanel _winScreen;

    [SetUp]
    public void SetUp()
    {
        _gameManagerObject = new GameObject("GameManager");
        _gameManagerObject.AddComponent<GameManager>();

        _panelObject = new GameObject();
        _winScreen = _panelObject.AddComponent<WinScreenPanel>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_gameManagerObject);
        Object.Destroy(_panelObject);
        typeof(GameManager).GetProperty("Instance").SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator OpenSettings_ShouldToggleActivePanels()
    {
        GameObject mainPanel = new GameObject();
        GameObject settingsPanel = new GameObject();
        typeof(WinScreenPanel).GetField("winMainPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_winScreen, mainPanel);
        typeof(WinScreenPanel).GetField("settingsPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_winScreen, settingsPanel);

        _winScreen.OpenSettings();
        yield return null;

        Assert.IsFalse(mainPanel.activeSelf);
        Assert.IsTrue(settingsPanel.activeSelf);

        Object.Destroy(mainPanel);
        Object.Destroy(settingsPanel);
    }
}
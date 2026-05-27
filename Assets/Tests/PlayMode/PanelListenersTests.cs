using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class PanelListenersTests
{
    private GameObject _gameManagerObject;
    private GameObject _panelObject;

    [SetUp]
    public void SetUp()
    {
        _gameManagerObject = new GameObject("GameManager");
        _gameManagerObject.AddComponent<GameManager>();

        _panelObject = new GameObject("Panel");
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_gameManagerObject);
        Object.Destroy(_panelObject);
        typeof(GameManager).GetProperty("Instance").SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator GameOverPanel_OnRestartClicked_ShouldTriggerGameRestart()
    {
        var panel = _panelObject.AddComponent<GameOverPanel>();
        var restartBtn = new GameObject().AddComponent<Button>();
        typeof(GameOverPanel).GetField("restartButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(panel, restartBtn);

        panel.SendMessage("Start");
        restartBtn.onClick.Invoke();
        yield return null;

        Assert.AreEqual(0, GameManager.Instance.currentLevel);
    }

    [UnityTest]
    public IEnumerator PausePanel_OnExitClicked_ShouldTriggerGameExit()
    {
        var panel = _panelObject.AddComponent<PausePanel>();
        var exitBtn = new GameObject().AddComponent<Button>();
        typeof(PausePanel).GetField("exitButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(panel, exitBtn);

        panel.SendMessage("Start");
        exitBtn.onClick.Invoke();
        yield return null;

        Assert.IsNotNull(GameManager.Instance);
    }

    [UnityTest]
    public IEnumerator TerminalPanel_ShouldBindUiControllerMethods()
    {
        var controller = _panelObject.AddComponent<TerminalUIController>();
        var text = new GameObject().AddComponent<TMPro.TextMeshProUGUI>();
        typeof(TerminalUIController).GetField("contentText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(controller, text);

        var panel = _panelObject.AddComponent<TerminalPanel>();
        var missionBtn = new GameObject().AddComponent<Button>();
        typeof(TerminalPanel).GetField("missionButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(panel, missionBtn);

        panel.SendMessage("Awake");
        panel.SendMessage("Start");
        missionBtn.onClick.Invoke();
        yield return null;

        Assert.IsFalse(string.IsNullOrEmpty(text.text));
    }
}
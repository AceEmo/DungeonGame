using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class BlackjackRewardSystemTests
{
    private GameObject _testHolder;
    private BlackjackRewardSystem _rewardSystem;
    private BlackjackUI _ui;

    [SetUp]
    public void SetUp()
    {
        _testHolder = new GameObject();
        _rewardSystem = _testHolder.AddComponent<BlackjackRewardSystem>();
        _ui = _testHolder.AddComponent<BlackjackUI>();

        var exitBtn = new GameObject("Exit").AddComponent<Button>();
        var resultTxt = new GameObject("Result").AddComponent<TextMeshProUGUI>();
        var uiType = typeof(BlackjackUI);
        var exitField = uiType.GetField("exitButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var resultField = uiType.GetField("resultText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (exitField != null)
        {
            exitField.SetValue(_ui, exitBtn);
        }
        else
        {
            var exitProp = uiType.GetProperty("exitButton", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (exitProp != null) exitProp.SetValue(_ui, exitBtn);
        }
        if (resultField != null)
        {
            resultField.SetValue(_ui, resultTxt);
        }
        else
        {
            var resultProp = uiType.GetProperty("resultText", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (resultProp != null) resultProp.SetValue(_ui, resultTxt);
        }
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_testHolder);
    }

    [UnityTest]
    public IEnumerator WinRoutine_ShouldEnableExitButtonAfterDelay()
    {
        var exitButtonField = typeof(BlackjackUI).GetField("exitButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var exitButton = exitButtonField != null ? exitButtonField.GetValue(_ui) as Button : null;
        
        Assert.IsNotNull(exitButton);
        exitButton.interactable = false;
        Transform spawnPoint = _testHolder.transform;

        _rewardSystem.StartCoroutine(_rewardSystem.WinRoutine(_ui, false, spawnPoint));
        
        yield return new WaitForSeconds(1.7f);

        Assert.IsTrue(exitButton.interactable);
    }
}
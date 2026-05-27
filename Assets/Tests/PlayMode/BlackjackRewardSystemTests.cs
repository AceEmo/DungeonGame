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

        _ui.exitButton = new GameObject("Exit").AddComponent<Button>();
        _ui.resultText = new GameObject("Result").AddComponent<TextMeshProUGUI>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_testHolder);
    }

    [UnityTest]
    public IEnumerator WinRoutine_ShouldEnableExitButtonAfterDelay()
    {
        _ui.exitButton.interactable = false;
        Transform spawnPoint = _testHolder.transform;

        _rewardSystem.StartCoroutine(_rewardSystem.WinRoutine(_ui, false, spawnPoint));
        
        yield return new WaitForSeconds(1.7f);

        Assert.IsTrue(_ui.exitButton.interactable);
    }
}
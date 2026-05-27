using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class UpgradePanelTests
{
    private GameObject _panelObject;
    private UpgradePanel _upgradePanel;
    private PlayerStats _stats;

    [SetUp]
    public void SetUp()
    {
        _panelObject = new GameObject();
        _upgradePanel = _panelObject.AddComponent<UpgradePanel>();

        _stats = ScriptableObject.CreateInstance<PlayerStats>();
        _upgradePanel.playerStats = _stats;

        _upgradePanel.healthButton = new GameObject().AddComponent<Button>();
        _upgradePanel.speedButton = new GameObject().AddComponent<Button>();
        _upgradePanel.fireRateButton = new GameObject().AddComponent<Button>();
        _upgradePanel.bulletSpeedButton = new GameObject().AddComponent<Button>();
        _upgradePanel.damageButton = new GameObject().AddComponent<Button>();
        var statsText = new GameObject().AddComponent<TextMeshProUGUI>();
        typeof(UpgradePanel)
            .GetField("statsText", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.SetValue(_upgradePanel, statsText);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_panelObject);
        Object.Destroy(_stats);
    }

    [UnityTest]
    public IEnumerator Refresh_ShouldUpdateTextAndButtonInteractableState()
    {
        _stats.scrap = 0;

        _upgradePanel.SendMessage("OnEnable");
        yield return null;

        Assert.IsFalse(_upgradePanel.healthButton.interactable);

        _stats.AddScrap(100);
        yield return null;

        Assert.IsTrue(_upgradePanel.healthButton.interactable);
    }

    [UnityTest]
    public IEnumerator UpgradeSpeed_WithEnoughScrap_ShouldConsumeScrapAndIncreaseStat()
    {
        _stats.scrap = 50;
        _stats.moveSpeed = 5f;

        _upgradePanel.UpgradeSpeed();
        yield return null;

        Assert.AreEqual(20, _stats.scrap);
        Assert.AreEqual(5.5f, _stats.moveSpeed);
    }

    [UnityTest]
    public IEnumerator UpgradeDamage_WithoutEnoughScrap_ShouldNotModifyStats()
    {
        _stats.scrap = 10;
        _stats.damage = 1;

        _upgradePanel.UpgradeDamage();
        yield return null;

        Assert.AreEqual(10, _stats.scrap);
        Assert.AreEqual(1, _stats.damage);
    }
}
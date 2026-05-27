using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class GameUiElementsTests
{
    private GameObject _uiObject;
    private PlayerStats _stats;

    [SetUp]
    public void SetUp()
    {
        _uiObject = new GameObject();
        _stats = ScriptableObject.CreateInstance<PlayerStats>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_uiObject);
        Object.Destroy(_stats);
        typeof(InteractionUI).GetProperty("Instance").SetValue(null, null);
    }

    [UnityTest]
    public IEnumerator ScrapUI_OnEnable_ShouldSyncTextWithPlayerStats()
    {
        var text = _uiObject.AddComponent<TextMeshProUGUI>();
        var scrapUi = _uiObject.AddComponent<ScrapUI>();
        typeof(ScrapUI).GetField("scrapText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(scrapUi, text);
        typeof(ScrapUI).GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(scrapUi, _stats);
        _stats.scrap = 42;

        scrapUi.SendMessage("OnEnable");
        yield return null;

        Assert.AreEqual("42", text.text);
    }

    [UnityTest]
    public IEnumerator InteractionUI_Awake_ShouldEstablishSingletonPattern()
    {
        var hintPanel = new GameObject();
        var interactionUi = _uiObject.AddComponent<InteractionUI>();
        typeof(InteractionUI).GetField("hintPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(interactionUi, hintPanel);

        interactionUi.SendMessage("Awake");
        yield return null;

        Assert.AreEqual(interactionUi, InteractionUI.Instance);
        Object.Destroy(hintPanel);
    }

    [UnityTest]
    public IEnumerator HeartUI_UpdateHearts_ShouldSpawnCorrectNumberOfPrefabs()
    {
        var panel = new GameObject().transform;
        var prefab = new GameObject();
        prefab.AddComponent<Image>();

        var heartUi = _uiObject.AddComponent<HeartUI>();
        typeof(HeartUI).GetField("panel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(heartUi, panel);
        typeof(HeartUI).GetField("heartPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(heartUi, prefab);

        heartUi.SendMessage("UpdateHearts", 6f);
        yield return null;

        Assert.AreEqual(3, panel.childCount);

        Object.Destroy(panel.gameObject);
        Object.Destroy(prefab);
    }
}
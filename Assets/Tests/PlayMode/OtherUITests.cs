using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class OtherUITests
{
    private GameObject uiObject;
    private PlayerStats testStats;

    [SetUp]
    public void Setup()
    {
        uiObject = new GameObject("UI");
        testStats = ScriptableObject.CreateInstance<PlayerStats>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(uiObject);
        Object.DestroyImmediate(testStats);
        
        FieldInfo instanceField = typeof(GameManager).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
        }
    }

    [Test]
    public void ScrapUIUpdatesTextWhenStatsChange()
    {
        ScrapUI scrapUI = uiObject.AddComponent<ScrapUI>();
        
        GameObject textObj = new GameObject("Text");
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        
        FieldInfo textField = typeof(ScrapUI).GetField("scrapText", BindingFlags.NonPublic | BindingFlags.Instance);
        textField.SetValue(scrapUI, tmpText);

        FieldInfo statsField = typeof(ScrapUI).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(scrapUI, testStats);

        MethodInfo onEnableMethod = typeof(ScrapUI).GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance);
        onEnableMethod.Invoke(scrapUI, null);

        testStats.AddScrap(15);

        Assert.AreEqual("15", tmpText.text);
    }

    [UnityTest]
    public IEnumerator InteractionUIShowHintActivatesPanelAndSetsText()
    {
        InteractionUI interactionUI = uiObject.AddComponent<InteractionUI>();

        GameObject panelObj = new GameObject("HintPanel");
        panelObj.SetActive(false);

        GameObject textObj = new GameObject("HintText");
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();

        FieldInfo panelField = typeof(InteractionUI).GetField("hintPanel", BindingFlags.NonPublic | BindingFlags.Instance);
        panelField.SetValue(interactionUI, panelObj);

        FieldInfo textField = typeof(InteractionUI).GetField("hintText", BindingFlags.NonPublic | BindingFlags.Instance);
        textField.SetValue(interactionUI, tmpText);

        GameObject cameraObj = new GameObject("MainCamera");
        Camera mainCamera = cameraObj.AddComponent<Camera>();
        cameraObj.tag = "MainCamera";
        
        FieldInfo cameraField = typeof(InteractionUI).GetField("mainCamera", BindingFlags.NonPublic | BindingFlags.Instance);
        cameraField.SetValue(interactionUI, mainCamera);

        interactionUI.ShowHint("[E] Test", Vector3.zero);

        yield return null;

        Assert.IsTrue(panelObj.activeSelf);
        Assert.AreEqual("[E] Test", tmpText.text);

        Object.DestroyImmediate(cameraObj);
    }
}
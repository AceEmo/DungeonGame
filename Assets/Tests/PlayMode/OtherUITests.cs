using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class OtherUITests
{
    private GameObject uiObject;

    [SetUp]
    public void Setup()
    {
        uiObject = new GameObject("UI");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(uiObject);
    }

    [Test]
    public void ScrapUIUpdatesTextWithManagerScrapValue()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        GameManager gameManager = gameManagerObject.AddComponent<GameManager>();
        gameManager.AddScrap(15);

        ScrapUI scrapUI = uiObject.AddComponent<ScrapUI>();
        
        GameObject textObj = new GameObject("Text");
        TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
        
        FieldInfo textField = typeof(ScrapUI).GetField("scrapText", BindingFlags.NonPublic | BindingFlags.Instance);
        textField.SetValue(scrapUI, tmpText);

        MethodInfo updateMethod = typeof(ScrapUI).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
        updateMethod.Invoke(scrapUI, null);

        Assert.AreEqual("15", tmpText.text);

        Object.DestroyImmediate(gameManagerObject);
        FieldInfo instanceField = typeof(GameManager).GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
        if (instanceField != null) instanceField.SetValue(null, null);
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

        Camera mainCamera = new GameObject("MainCamera").AddComponent<Camera>();
        mainCamera.tag = "MainCamera";
        
        FieldInfo cameraField = typeof(InteractionUI).GetField("mainCamera", BindingFlags.NonPublic | BindingFlags.Instance);
        cameraField.SetValue(interactionUI, mainCamera);

        interactionUI.ShowHint("[E] Test", Vector3.zero);

        yield return null;

        Assert.IsTrue(panelObj.activeSelf);
        Assert.AreEqual("[E] Test", tmpText.text);

        Object.DestroyImmediate(mainCamera.gameObject);
    }
}
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class PanelUITests
{
    private GameObject gameManagerObject;
    private GameManager gameManager;
    private GameObject panelObject;

    [SetUp]
    public void Setup()
    {
        gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();
        
        MethodInfo initMethod = typeof(GameManager).GetMethod("InitializeSceneData", BindingFlags.NonPublic | BindingFlags.Instance);
        initMethod.Invoke(gameManager, null);

        panelObject = new GameObject("Panel");
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(panelObject);
        if (gameManagerObject != null)
        {
            Object.DestroyImmediate(gameManagerObject);
        }
        
        FieldInfo instanceField = typeof(GameManager).GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
        }
    }

    [UnityTest]
    public IEnumerator GameOverPanelRestartButtonInvokesGameManagerRestart()
    {
        GameOverPanel gameOverPanel = panelObject.AddComponent<GameOverPanel>();
        
        GameObject restartBtnObj = new GameObject("RestartBtn");
        Button restartButton = restartBtnObj.AddComponent<Button>();
        
        FieldInfo restartField = typeof(GameOverPanel).GetField("restartButton", BindingFlags.NonPublic | BindingFlags.Instance);
        restartField.SetValue(gameOverPanel, restartButton);

        MethodInfo startMethod = typeof(GameOverPanel).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(gameOverPanel, null);

        gameManager.AddScrap(10);
        
        restartButton.onClick.Invoke();
        yield return null;

        Assert.AreEqual(0, gameManager.Scrap);

        Object.DestroyImmediate(restartBtnObj);
    }

    [UnityTest]
    public IEnumerator PausePanelRestartButtonInvokesGameManagerRestart()
    {
        PausePanel pausePanel = panelObject.AddComponent<PausePanel>();
        
        GameObject restartBtnObj = new GameObject("RestartBtn");
        Button restartButton = restartBtnObj.AddComponent<Button>();
        
        FieldInfo restartField = typeof(PausePanel).GetField("restartButton", BindingFlags.NonPublic | BindingFlags.Instance);
        restartField.SetValue(pausePanel, restartButton);

        MethodInfo startMethod = typeof(PausePanel).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        startMethod.Invoke(pausePanel, null);

        gameManager.AddScrap(5);
        
        restartButton.onClick.Invoke();
        yield return null;

        Assert.AreEqual(0, gameManager.Scrap);

        Object.DestroyImmediate(restartBtnObj);
    }
}
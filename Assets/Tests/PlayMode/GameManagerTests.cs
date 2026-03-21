using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameManagerTests
{
    private GameObject gameManagerObj;
    private GameManager gameManager;

    [SetUp]
    public void Setup()
    {
        gameManagerObj = new GameObject("GameManager");
        gameManager = gameManagerObj.AddComponent<GameManager>();
        Time.timeScale = 1f;
    }

    [TearDown]
    public void Teardown()
    {
        Time.timeScale = 1f;
        
        if (gameManagerObj != null)
        {
            Object.DestroyImmediate(gameManagerObj);
        }

        FieldInfo instanceField = typeof(GameManager).GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
        }
    }

    [UnityTest]
    public IEnumerator SingletonDestroysDuplicateInstance()
    {
        yield return null;

        GameObject duplicateObj = new GameObject("DuplicateGameManager");
        duplicateObj.AddComponent<GameManager>();

        yield return null;

        Assert.IsTrue(duplicateObj == null);
    }

    [Test]
    public void AddScrapIncreasesTotalAmount()
    {
        gameManager.AddScrap(10);
        gameManager.AddScrap(5);

        Assert.AreEqual(15, gameManager.Scrap);
    }

    [Test]
    public void ResetRunSetsScrapToZero()
    {
        gameManager.AddScrap(20);
        
        gameManager.ResetRun();

        Assert.AreEqual(0, gameManager.Scrap);
    }

    [Test]
    public void IsGameplayActiveReturnsTrueInitially()
    {
        MethodInfo initMethod = typeof(GameManager).GetMethod("InitializeSceneData", BindingFlags.NonPublic | BindingFlags.Instance);
        initMethod.Invoke(gameManager, null);

        Assert.IsTrue(gameManager.IsGameplayActive());
        Assert.AreEqual(1f, Time.timeScale);
    }

    [Test]
    public void OpenBlackjackChangesStateAndDisablesGameplay()
    {
        gameManager.OpenBlackjack();

        Assert.IsFalse(gameManager.IsGameplayActive());
    }

    [Test]
    public void CloseBlackjackRestoresGameplayState()
    {
        gameManager.OpenBlackjack();
        gameManager.CloseBlackjack();

        Assert.IsTrue(gameManager.IsGameplayActive());
    }

    [Test]
    public void RegisterBlackjackCanvasAssignsCanvasProperly()
    {
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.SetActive(false);

        gameManager.RegisterBlackjackCanvas(canvasObj);
        gameManager.OpenBlackjack();

        Assert.IsTrue(canvasObj.activeSelf);

        Object.DestroyImmediate(canvasObj);
    }

    [Test]
    public void HandleGameOverSetsTimeScaleToZero()
    {
        MethodInfo gameOverMethod = typeof(GameManager).GetMethod("HandleGameOver", BindingFlags.NonPublic | BindingFlags.Instance);
        
        gameOverMethod.Invoke(gameManager, null);

        Assert.AreEqual(0f, Time.timeScale);
    }
}
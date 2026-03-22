using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTests
{
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private PlayerStats testStats;

    [SetUp]
    public void Setup()
    {
        gameManagerObj = new GameObject("GameManager");
        gameManager = gameManagerObj.AddComponent<GameManager>();
        
        testStats = ScriptableObject.CreateInstance<PlayerStats>();
        gameManager.playerStats = testStats;
        
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

        if (testStats != null)
        {
            Object.DestroyImmediate(testStats);
        }

        FieldInfo instanceField = typeof(GameManager).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic);
        if (instanceField == null)
        {
            instanceField = typeof(GameManager).GetField("<Instance>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
        }

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
    public void RestartGameResetsPlayerStats()
    {
        testStats.scrap = 50;
        testStats.damage = 99;

        gameManager.RestartGame();

        Assert.AreEqual(0, testStats.scrap);
        Assert.AreEqual(testStats.baseDamage, testStats.damage);
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
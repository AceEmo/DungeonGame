using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTests
{
    private GameObject _gameManagerObject;
    private GameManager _gameManager;

    [SetUp]
    public void SetUp()
    {
        _gameManagerObject = new GameObject("GameManager");
        _gameManager = _gameManagerObject.AddComponent<GameManager>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_gameManagerObject != null)
        {
            Object.Destroy(_gameManagerObject);
        }
    }

    [UnityTest]
    public IEnumerator Awake_ShouldSetInstanceAndInitializeSettings()
    {
        yield return null;

        Assert.AreEqual(_gameManager, GameManager.Instance);
        Assert.IsNotNull(_gameManager.Settings);
    }

    [UnityTest]
    public IEnumerator Awake_WhenDuplicateInstanceExists_ShouldDestroyDuplicate()
    {
        GameObject duplicateObject = new GameObject("DuplicateGameManager");
        GameManager duplicateManager = duplicateObject.AddComponent<GameManager>();

        yield return null;

        Assert.IsTrue(duplicateObject == null);
    }

    [UnityTest]
    public IEnumerator OpenTerminal_ShouldChangeStateToTerminal()
    {
        yield return null;

        _gameManager.OpenTerminal();

        Assert.AreEqual(GameState.Terminal, _gameManager.CurrentState);
    }

    [UnityTest]
    public IEnumerator CloseTerminal_ShouldReturnStateToGameplay()
    {
        yield return null;
        _gameManager.OpenTerminal();

        _gameManager.CloseTerminal();

        Assert.AreEqual(GameState.Gameplay, _gameManager.CurrentState);
    }

    [UnityTest]
    public IEnumerator IsGameplayActive_WhenInGameplayState_ShouldReturnTrue()
    {
        yield return null;
        _gameManager.CloseTerminal();

        bool isActive = _gameManager.IsGameplayActive();

        Assert.IsTrue(isActive);
    }

    [UnityTest]
    public IEnumerator HandleGameWin_ShouldSetStateToWinScreen()
    {
        yield return null;

        _gameManager.HandleGameWin();

        Assert.AreEqual(GameState.WinScreen, _gameManager.CurrentState);
    }
}
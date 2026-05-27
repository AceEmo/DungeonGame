using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class GameStateManagerTests
{
    private GameStateManager _stateManager;
    private PanelController _mockPanelController;

    [SetUp]
    public void SetUp()
    {
        _mockPanelController = new PanelController();
        _stateManager = new GameStateManager(_mockPanelController);
    }

    [Test]
    public void SetState_ShouldUpdateCurrentState()
    {
        _stateManager.SetState(GameState.Gameplay);

        Assert.AreEqual(GameState.Gameplay, _stateManager.CurrentState);
    }

    [Test]
    public void SetState_WhenPaused_ShouldSetTimeScaleToZero()
    {
        _stateManager.SetState(GameState.Paused);

        Assert.AreEqual(0f, Time.timeScale);
    }

    [Test]
    public void SetState_WhenGameplay_ShouldSetTimeScaleToOne()
    {
        _stateManager.SetState(GameState.Paused);
        _stateManager.SetState(GameState.Gameplay);

        Assert.AreEqual(1f, Time.timeScale);
    }

    [Test]
    public void SetState_WhenGameplay_ShouldLockCursor()
    {
        _stateManager.SetState(GameState.Gameplay);

        Assert.AreEqual(CursorLockMode.Locked, Cursor.lockState);
        Assert.IsFalse(Cursor.visible);
    }

    [Test]
    public void SetState_WhenUIState_ShouldUnlockCursor()
    {
        _stateManager.SetState(GameState.MainMenu);

        Assert.AreEqual(CursorLockMode.None, Cursor.lockState);
        Assert.IsTrue(Cursor.visible);
    }
}
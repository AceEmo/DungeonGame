using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PanelControllerTests
{
    private PanelController _panelController;
    private GameObject _pausePanel;
    private GameObject _gameOverPanel;

    [SetUp]
    public void SetUp()
    {
        _panelController = new PanelController();
        _pausePanel = new GameObject("PausePanel");
        _gameOverPanel = new GameObject("GameOverPanel");
        
        _panelController.PausePanel = _pausePanel;
        _panelController.GameOverPanel = _gameOverPanel;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_pausePanel);
        Object.DestroyImmediate(_gameOverPanel);
    }

    [Test]
    public void UpdateUIStates_WhenStateIsPaused_ShouldActivatePausePanelAndDeactivateOthers()
    {
        _pausePanel.SetActive(false);
        _gameOverPanel.SetActive(true);

        _panelController.UpdateUIStates(GameState.Paused);

        Assert.IsTrue(_pausePanel.activeSelf);
        Assert.IsFalse(_gameOverPanel.activeSelf);
    }

    [Test]
    public void UpdateUIStates_WhenStateIsGameOver_ShouldActivateGameOverPanelAndDeactivateOthers()
    {
        _pausePanel.SetActive(true);
        _gameOverPanel.SetActive(false);

        _panelController.UpdateUIStates(GameState.GameOver);

        Assert.IsFalse(_pausePanel.activeSelf);
        Assert.IsTrue(_gameOverPanel.activeSelf);
    }
}
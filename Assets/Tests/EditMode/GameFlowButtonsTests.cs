using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

[TestFixture]
public class GameFlowButtonsTests
{
    private GameObject _gameManagerObject;

    [SetUp]
    public void SetUp()
    {
        _gameManagerObject = new GameObject("GameManager");
        var manager = _gameManagerObject.AddComponent<GameManager>();
        
        var instanceField = typeof(GameManager).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) 
                            ?? typeof(GameManager).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        
        if (instanceField != null)
        {
            instanceField.SetValue(null, manager);
        }
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameManagerObject);
        
        var instanceField = typeof(GameManager).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) 
                            ?? typeof(GameManager).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
        }
    }

    [Test]
    public void BindRestartButton_ShouldResetCurrentLevel()
    {
        var buttonObject = new GameObject("Restart");
        var button = buttonObject.AddComponent<Button>();

        GameFlowButtons.Bind(button, null);
        button.onClick.Invoke();

        Assert.AreEqual(0, GameManager.Instance.currentLevel);

        GameFlowButtons.Unbind(button, null);
        Object.DestroyImmediate(buttonObject);
    }
}
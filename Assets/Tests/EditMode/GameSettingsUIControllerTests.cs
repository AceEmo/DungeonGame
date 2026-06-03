using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

[TestFixture]
public class GameSettingsUIControllerTests
{
    private GameObject _gameManagerObject;
    private Slider _levelsSlider;
    private Slider _difficultySlider;

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

        _levelsSlider = new GameObject("LevelsSlider").AddComponent<Slider>();
        _difficultySlider = new GameObject("DifficultySlider").AddComponent<Slider>();
    }

    [TearDown]
    public void TearDown()
    {
        UnityEngine.Object.DestroyImmediate(_gameManagerObject);
        UnityEngine.Object.DestroyImmediate(_levelsSlider.gameObject);
        UnityEngine.Object.DestroyImmediate(_difficultySlider.gameObject);
        
        var instanceField = typeof(GameManager).GetField("instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public) 
                            ?? typeof(GameManager).GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (instanceField != null)
        {
            instanceField.SetValue(null, null);
        }
    }

    [Test]
    public void Initialize_ShouldApplyTemporarySettingsToSliders()
    {
        var controller = CreateController();

        controller.GetType().GetMethod("Initialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(controller, null);

        Assert.AreEqual(GameManager.Instance.Settings.MaxLevels, _levelsSlider.value);
        Assert.AreEqual((int)GameManager.Instance.Settings.Difficulty, _difficultySlider.value);
    }

    [Test]
    public void SaveToGameManager_ShouldPersistTemporarySettings()
    {
        var controller = CreateController();
        controller.GetType().GetMethod("Initialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(controller, null);

        _levelsSlider.onValueChanged.Invoke(GameSettings.MaxLevelsLimit);
        _difficultySlider.onValueChanged.Invoke((int)GameDifficulty.Hard);

        controller.GetType().GetMethod("SaveToGameManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(controller, null);

        Assert.AreEqual(GameSettings.MaxLevelsLimit, GameManager.Instance.Settings.MaxLevels);
        Assert.AreEqual(GameDifficulty.Hard, GameManager.Instance.Settings.Difficulty);
    }

    private object CreateController()
    {
        Type type = null;
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType("GameSettingsUIController");
            if (type != null) break;
        }

        if (type == null)
        {
            throw new InvalidOperationException("GameSettingsUIController type not found.");
        }

        return Activator.CreateInstance(type, _levelsSlider, _difficultySlider, null, null)
               ?? throw new InvalidOperationException("Failed to create GameSettingsUIController.");
    }
}
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class GameSettingsAndDifficultyTests
{
    [SetUp]
    public void SetUp()
    {
        PlayerPrefs.DeleteAll();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerPrefs.DeleteAll();
    }

    [Test]
    public void GetStatMultiplier_ShouldReturnCorrectValuesForDifficulty()
    {
        float easyMultiplier = GameDifficulty.Easy.GetStatMultiplier();
        float normalMultiplier = GameDifficulty.Normal.GetStatMultiplier();
        float hardMultiplier = GameDifficulty.Hard.GetStatMultiplier();

        Assert.AreEqual(0.75f, easyMultiplier);
        Assert.AreEqual(1.00f, normalMultiplier);
        Assert.AreEqual(1.25f, hardMultiplier);
    }

    [Test]
    public void SaveToPrefs_ShouldStoreSettingsInPlayerPrefs()
    {
        GameSettings settings = new GameSettings();
        settings.MaxLevels = 8;
        settings.Difficulty = GameDifficulty.Hard;

        settings.SaveToPrefs();

        Assert.AreEqual(8, PlayerPrefs.GetInt("MaxLevels"));
        Assert.AreEqual((int)GameDifficulty.Hard, PlayerPrefs.GetInt("Difficulty"));
    }

    [Test]
    public void LoadFromPrefs_WhenNoDataExists_ShouldLoadDefaults()
    {
        GameSettings settings = new GameSettings();

        settings.LoadFromPrefs();

        Assert.AreEqual(GameSettings.DefaultLevels, settings.MaxLevels);
        Assert.AreEqual(GameDifficulty.Normal, settings.Difficulty);
    }

    [Test]
    public void LoadFromPrefs_ShouldClampValuesWithinLimits()
    {
        PlayerPrefs.SetInt("MaxLevels", 100);
        GameSettings settings = new GameSettings();

        settings.LoadFromPrefs();

        Assert.AreEqual(GameSettings.MaxLevelsLimit, settings.MaxLevels);
    }

    [Test]
    public void CopyConstructor_ShouldCreateIdenticalCopy()
    {
        GameSettings original = new GameSettings();
        original.MaxLevels = 6;
        original.Difficulty = GameDifficulty.Easy;

        GameSettings copy = new GameSettings(original);

        Assert.AreEqual(original.MaxLevels, copy.MaxLevels);
        Assert.AreEqual(original.Difficulty, copy.Difficulty);
    }
}
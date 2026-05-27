using NUnit.Framework;

[TestFixture]
public class LevelProgressionManagerTests
{
    private LevelProgressionManager _progressionManager;

    [SetUp]
    public void SetUp()
    {
        _progressionManager = new LevelProgressionManager();
    }

    [Test]
    public void ResetLevels_ShouldSetCurrentLevelToZero()
    {
        _progressionManager.ResetLevels();

        Assert.AreEqual(0, _progressionManager.CurrentLevel);
    }

    [Test]
    public void MaxLevels_ShouldStoreAndReturnCorrectValue()
    {
        _progressionManager.MaxLevels = 10;

        Assert.AreEqual(10, _progressionManager.MaxLevels);
    }
}
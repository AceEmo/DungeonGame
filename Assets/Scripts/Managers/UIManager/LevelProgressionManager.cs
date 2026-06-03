using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressionManager
{
    private int currentLevel = 0;

    public int MaxLevels { get; set; } = GameSettings.DefaultLevels;
    public int CurrentLevel => currentLevel;

    public void ResetLevels()
    {
        currentLevel = 0;
    }

    public void LoadNextLevel()
    {
        currentLevel++;

        if (currentLevel > MaxLevels)
        {
            SceneManager.LoadScene(SceneNames.WinScreen);
            return;
        }

        SceneManager.LoadScene(SceneNames.GetLevelName(currentLevel));
    }
}
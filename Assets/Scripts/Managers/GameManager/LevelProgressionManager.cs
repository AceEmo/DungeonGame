using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressionManager
{
    private int currentLevel = 0;
    private const int MaxLevels = 5;

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
            SceneManager.LoadScene("WinScreen");
            return;
        }

        SceneManager.LoadScene("Level" + currentLevel);
    }
}
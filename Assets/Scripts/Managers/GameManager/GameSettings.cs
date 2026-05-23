using UnityEngine;

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard
}

public class GameSettings
{
    public const int MinLevelsLimit = 3;
    public const int MaxLevelsLimit = 13;
    public const int DefaultLevels = 5;

    public int MaxLevels { get; set; } = DefaultLevels;
    public GameDifficulty Difficulty { get; set; } = GameDifficulty.Normal;

    public GameSettings() { }

    public GameSettings(GameSettings source)
    {
        if (source == null) return;

        MaxLevels = source.MaxLevels;
        Difficulty = source.Difficulty;
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetInt("MaxLevels", MaxLevels);
        PlayerPrefs.SetInt("Difficulty", (int)Difficulty);
        PlayerPrefs.Save();
    }

    public void LoadFromPrefs()
    {
        MaxLevels = PlayerPrefs.GetInt("MaxLevels", DefaultLevels);
        Difficulty = (GameDifficulty)PlayerPrefs.GetInt("Difficulty", (int)GameDifficulty.Normal);

        MaxLevels = Mathf.Clamp(MaxLevels, MinLevelsLimit, MaxLevelsLimit);
    }
}
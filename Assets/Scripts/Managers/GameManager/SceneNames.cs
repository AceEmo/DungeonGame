public static class SceneNames
{
    public const string MainMenu = "MainMenu";
    public const string HubRoom = "HubRoom";
    public const string WinScreen = "WinScreen";
    public const string LevelPrefix = "Level";

    public static string GetLevelName(int level)
    {
        return $"{LevelPrefix}{level}";
    }
}

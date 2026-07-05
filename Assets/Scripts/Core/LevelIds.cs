/// <summary>
/// Scene name constants, used instead of magic strings so a future level-select screen can just
/// iterate AllLevels.
/// </summary>
public static class LevelIds
{
    public const string MainMenu = "MainMenu";
    public const string LevelSelect = "LevelSelect";
    public const string Settings = "Settings";
    public const string Level01 = "Level_01";
    public const string Level02 = "Level_02";
    public const string Level03 = "Level_03";
    public const string Level04 = "Level_04";
    public const string Level05 = "Level_05";
    public const string Level06 = "Level_06";

    public static readonly string[] AllLevels = { Level01, Level02, Level03, Level04, Level05, Level06 };

    /// <summary>Returns the scene name after currentSceneName in AllLevels, or null if it's the last one.</summary>
    public static string GetNextLevel(string currentSceneName)
    {
        int index = System.Array.IndexOf(AllLevels, currentSceneName);
        if (index < 0 || index >= AllLevels.Length - 1) return null;
        return AllLevels[index + 1];
    }
}

using UnityEngine;

/// <summary>
/// PlayerPrefs-backed progress tracking: which levels are completed, star rating, best time, and
/// best ammo usage per level. A level unlocks automatically once the previous one is completed
/// (the first level in LevelIds.AllLevels is always unlocked).
/// </summary>
public static class ProgressManager
{
    private const string CompletedKey = "level_completed_";
    private const string StarsKey = "level_stars_";
    private const string BestTimeKey = "level_besttime_";
    private const string BestUsedKey = "level_bestused_";

    public static bool IsUnlocked(string levelId)
    {
        int index = System.Array.IndexOf(LevelIds.AllLevels, levelId);
        if (index <= 0) return true;
        return IsCompleted(LevelIds.AllLevels[index - 1]);
    }

    public static bool IsCompleted(string levelId) => PlayerPrefs.GetInt(CompletedKey + levelId, 0) == 1;

    public static int GetStars(string levelId) => PlayerPrefs.GetInt(StarsKey + levelId, 0);

    public static float GetBestTime(string levelId) => PlayerPrefs.GetFloat(BestTimeKey + levelId, -1f);

    public static int GetBestUsedBalls(string levelId) => PlayerPrefs.GetInt(BestUsedKey + levelId, -1);

    /// <summary>Records a level completion, keeping the best stars/time/ammo-usage seen so far.</summary>
    public static void RecordCompletion(string levelId, int stars, float timeSeconds, int usedBalls)
    {
        PlayerPrefs.SetInt(CompletedKey + levelId, 1);

        if (stars > GetStars(levelId)) PlayerPrefs.SetInt(StarsKey + levelId, stars);

        float bestTime = GetBestTime(levelId);
        if (bestTime < 0f || timeSeconds < bestTime) PlayerPrefs.SetFloat(BestTimeKey + levelId, timeSeconds);

        int bestUsed = GetBestUsedBalls(levelId);
        if (bestUsed < 0 || usedBalls < bestUsed) PlayerPrefs.SetInt(BestUsedKey + levelId, usedBalls);

        PlayerPrefs.Save();
    }

    /// <summary>1-3 stars based on how much of the ammo budget was left over on completion.</summary>
    public static int CalculateStars(int usedBalls, int maxBalls)
    {
        if (maxBalls <= 0) return 1;
        float usedRatio = (float)usedBalls / maxBalls;
        if (usedRatio <= 0.5f) return 3;
        if (usedRatio <= 0.8f) return 2;
        return 1;
    }

    public static void ResetAllProgress()
    {
        foreach (string levelId in LevelIds.AllLevels)
        {
            PlayerPrefs.DeleteKey(CompletedKey + levelId);
            PlayerPrefs.DeleteKey(StarsKey + levelId);
            PlayerPrefs.DeleteKey(BestTimeKey + levelId);
            PlayerPrefs.DeleteKey(BestUsedKey + levelId);
        }
        PlayerPrefs.Save();
    }
}

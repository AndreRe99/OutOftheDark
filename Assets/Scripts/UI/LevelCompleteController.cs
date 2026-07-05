using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Shown by LevelGoal once the light ray reaches the goal. Records progress (stars/time/ammo
/// used) via ProgressManager, which also unlocks the next level, and offers to continue to it or
/// return to the main menu.
/// </summary>
public class LevelCompleteController : MonoBehaviour
{
    public GameObject panel;
    public Text statsText;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance?.PlayLevelComplete();

        string levelId = SceneManager.GetActiveScene().name;
        ShotBudget budget = FindObjectOfType<ShotBudget>();
        GameHUD hud = FindObjectOfType<GameHUD>();

        int usedBalls = budget != null ? budget.Used : 0;
        int maxBalls = budget != null ? budget.maxShots : 1;
        float elapsed = hud != null ? hud.ElapsedSeconds : 0f;
        int stars = ProgressManager.CalculateStars(usedBalls, maxBalls);

        ProgressManager.RecordCompletion(levelId, stars, elapsed, usedBalls);

        if (statsText != null)
        {
            string starsDisplay = new string('*', stars) + new string('-', 3 - stars);
            string timeDisplay = hud != null ? hud.FormatElapsed() : "--:--";
            statsText.text = $"Sterne: {starsDisplay}   Lichtbälle verwendet: {usedBalls}/{maxBalls}   Zeit: {timeDisplay}";
        }
    }

    public void OnNextLevelButtonPressed()
    {
        string next = LevelIds.GetNextLevel(SceneManager.GetActiveScene().name);
        SceneLoader.LoadScene(next ?? LevelIds.MainMenu);
    }

    public void OnMainMenuButtonPressed()
    {
        SceneLoader.LoadScene(LevelIds.MainMenu);
    }
}

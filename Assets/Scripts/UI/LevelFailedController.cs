using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shown by ShotBudget once the light ball budget is exhausted without reaching the goal.
/// </summary>
public class LevelFailedController : MonoBehaviour
{
    public GameObject panel;
    public Text usedBallsText;
    public Text timeText;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance?.PlayLevelFailed();

        ShotBudget budget = FindObjectOfType<ShotBudget>();
        if (usedBallsText != null && budget != null)
        {
            usedBallsText.text = $"Verwendete Lichtbälle: {budget.Used}";
        }

        GameHUD hud = FindObjectOfType<GameHUD>();
        if (timeText != null && hud != null)
        {
            timeText.text = $"Benötigte Zeit: {hud.FormatElapsed()}";
        }
    }

    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f;

        CheckpointManager checkpoint = FindObjectOfType<CheckpointManager>();
        if (checkpoint != null && checkpoint.HasCheckpoint)
        {
            checkpoint.RespawnAtCheckpoint();
            if (panel != null) panel.SetActive(false);
            return;
        }

        SceneLoader.ReloadCurrentScene();
    }

    public void OnMainMenuButtonPressed() => SceneLoader.LoadScene(LevelIds.MainMenu);
}

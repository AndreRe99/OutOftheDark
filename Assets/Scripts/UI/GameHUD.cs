using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Top-of-screen HUD: remaining/used light balls, current level name, elapsed time, pause button.
/// Subscribes to ShotBudget.OnShotsChanged so the counters update after every shot.
/// </summary>
public class GameHUD : MonoBehaviour
{
    public ShotBudget shotBudget;
    public PauseMenuController pauseMenu;
    public Text remainingText;
    public Text usedText;
    public Text levelNameText;
    public Text timerText;

    private float elapsed;

    public float ElapsedSeconds => elapsed;

    public string FormatElapsed()
    {
        int minutes = Mathf.FloorToInt(elapsed / 60f);
        int seconds = Mathf.FloorToInt(elapsed % 60f);
        return $"{minutes:00}:{seconds:00}";
    }

    void Start()
    {
        if (levelNameText != null) levelNameText.text = SceneManager.GetActiveScene().name;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameplayMusic);
            AudioManager.Instance.PlayAmbient(AudioManager.Instance.ambientLoop);
        }

        if (shotBudget != null)
        {
            shotBudget.OnShotsChanged += HandleShotsChanged;
            HandleShotsChanged(shotBudget.Remaining, shotBudget.maxShots);
        }
    }

    void OnDestroy()
    {
        if (shotBudget != null) shotBudget.OnShotsChanged -= HandleShotsChanged;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        if (timerText != null) timerText.text = FormatElapsed();
    }

    private void HandleShotsChanged(int remaining, int max)
    {
        if (remainingText != null) remainingText.text = $"Verbleibend: {remaining}";
        if (usedText != null) usedText.text = $"Verwendet: {max - remaining}";
    }

    public void OnPauseButtonPressed()
    {
        if (pauseMenu != null) pauseMenu.TogglePause();
    }
}

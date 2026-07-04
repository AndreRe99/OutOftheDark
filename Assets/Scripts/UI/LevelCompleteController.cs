using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Shown by LevelGoal once the light ray reaches the goal. Offers to continue to the next level
/// (per LevelIds ordering) or return to the main menu.
/// </summary>
public class LevelCompleteController : MonoBehaviour
{
    public GameObject panel;

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
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

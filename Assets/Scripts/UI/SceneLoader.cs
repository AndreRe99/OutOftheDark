using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Small scene-transition utility. Always resets Time.timeScale so a pause or a "pauseOnWin"
/// freeze never leaks into the next scene.
/// </summary>
public static class SceneLoader
{
    public static void LoadScene(string sceneName)
    {
        AudioManager.Instance?.PlayMenuClick();
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public static void ReloadCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void QuitApplication()
    {
        Application.Quit();
    }
}

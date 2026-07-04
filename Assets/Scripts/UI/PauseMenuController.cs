using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pausePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        bool willPause = !pausePanel.activeSelf;
        pausePanel.SetActive(willPause);
        Time.timeScale = willPause ? 0f : 1f;
    }

    public void OnResumeButtonPressed() => TogglePause();

    public void OnRestartButtonPressed() => SceneLoader.ReloadCurrentScene();

    public void OnMainMenuButtonPressed() => SceneLoader.LoadScene(LevelIds.MainMenu);
}

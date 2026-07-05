using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
    }

    public void OnStartButtonPressed() => SceneLoader.LoadScene(LevelIds.Level01);

    public void OnLevelSelectButtonPressed() => SceneLoader.LoadScene(LevelIds.LevelSelect);

    public void OnSettingsButtonPressed() => SceneLoader.LoadScene(LevelIds.Settings);

    public void OnQuitButtonPressed() => SceneLoader.QuitApplication();
}

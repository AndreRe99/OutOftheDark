using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnStartButtonPressed() => SceneLoader.LoadScene(LevelIds.Level01);

    public void OnQuitButtonPressed() => SceneLoader.QuitApplication();
}

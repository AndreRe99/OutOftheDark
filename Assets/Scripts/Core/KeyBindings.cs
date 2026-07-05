using UnityEngine;

/// <summary>
/// Small rebindable-key registry backed by PlayerPrefs. Currently covers the Pause action - add
/// more named keys here as additional actions become rebindable.
/// </summary>
public static class KeyBindings
{
    private const string PauseKeyPref = "keybind_pause";

    public static KeyCode PauseKey
    {
        get => (KeyCode)PlayerPrefs.GetInt(PauseKeyPref, (int)KeyCode.Escape);
        set
        {
            PlayerPrefs.SetInt(PauseKeyPref, (int)value);
            PlayerPrefs.Save();
        }
    }
}

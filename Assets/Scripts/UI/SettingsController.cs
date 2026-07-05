using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Settings screen: music/SFX volume, fullscreen, resolution, brightness, language, pause-key
/// rebinding, and progress reset. Values persist via PlayerPrefs (through AudioManager for audio)
/// and apply immediately.
/// </summary>
public class SettingsController : MonoBehaviour
{
    private const string BrightnessKey = "settings_brightness";
    private const string LanguageKey = "settings_language";
    private const string FullscreenKey = "settings_fullscreen";

    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider brightnessSlider;
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown languageDropdown;
    public Text pauseKeyLabel;

    private Resolution[] resolutions;
    private bool waitingForKeybind;

    void Start()
    {
        SetupAudioControls();
        SetupBrightness();
        SetupFullscreen();
        SetupResolutions();
        SetupLanguage();
        UpdatePauseKeyLabel();
    }

    void Update()
    {
        if (!waitingForKeybind) return;

        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                KeyBindings.PauseKey = keyCode;
                waitingForKeybind = false;
                UpdatePauseKeyLabel();
                break;
            }
        }
    }

    private void SetupAudioControls()
    {
        if (AudioManager.Instance == null) return;

        if (musicSlider != null)
        {
            musicSlider.value = AudioManager.Instance.MusicVolume;
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = AudioManager.Instance.SfxVolume;
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
        }
    }

    private void SetupBrightness()
    {
        float brightness = PlayerPrefs.GetFloat(BrightnessKey, 1f);
        if (brightnessSlider != null)
        {
            brightnessSlider.value = brightness;
            brightnessSlider.onValueChanged.AddListener(SetBrightness);
        }
        ApplyBrightness(brightness);
    }

    public void SetBrightness(float value)
    {
        PlayerPrefs.SetFloat(BrightnessKey, value);
        ApplyBrightness(value);
    }

    private void ApplyBrightness(float value)
    {
        BrightnessOverlay.Instance?.Apply(value);
    }

    private void SetupFullscreen()
    {
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        Screen.fullScreen = fullscreen;
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = fullscreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
    }

    private void SetupResolutions()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        var options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        if (resolutions == null || index < 0 || index >= resolutions.Length) return;
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    private void SetupLanguage()
    {
        if (languageDropdown == null) return;

        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string> { "Deutsch", "English" });
        languageDropdown.value = PlayerPrefs.GetInt(LanguageKey, 0);
        languageDropdown.onValueChanged.AddListener(SetLanguage);
    }

    public void SetLanguage(int index)
    {
        // Hook point for a future full localization pass; currently just persists the choice.
        PlayerPrefs.SetInt(LanguageKey, index);
    }

    public void OnRebindPauseKeyButtonPressed()
    {
        waitingForKeybind = true;
        if (pauseKeyLabel != null) pauseKeyLabel.text = "Taste drücken...";
    }

    private void UpdatePauseKeyLabel()
    {
        if (pauseKeyLabel != null) pauseKeyLabel.text = $"Pause-Taste: {KeyBindings.PauseKey}";
    }

    public void OnResetProgressButtonPressed()
    {
        ProgressManager.ResetAllProgress();
    }

    public void OnBackButtonPressed() => SceneLoader.LoadScene(LevelIds.MainMenu);
}

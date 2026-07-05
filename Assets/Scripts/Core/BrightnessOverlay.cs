using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Persistent screen-darkening overlay (survives scene loads) driven by the brightness setting.
/// Kept separate from ScreenFader so the two don't fight over the same Image's alpha (ScreenFader
/// animates transiently during scene transitions; this one is a stable per-session setting).
/// </summary>
public class BrightnessOverlay : MonoBehaviour
{
    public static BrightnessOverlay Instance { get; private set; }

    private const string BrightnessKey = "settings_brightness";

    private Image overlay;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (transform.parent == null) DontDestroyOnLoad(gameObject);
        overlay = GetComponentInChildren<Image>();
        Apply(PlayerPrefs.GetFloat(BrightnessKey, 1f));
    }

    public void Apply(float brightness)
    {
        if (overlay == null) return;
        float darkenAlpha = Mathf.Clamp01(1f - brightness) * 0.85f;
        overlay.color = new Color(0f, 0f, 0f, darkenAlpha);
    }
}

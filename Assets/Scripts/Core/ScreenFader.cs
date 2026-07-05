using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Persistent full-screen fade overlay (survives scene loads): fades in from black whenever a
/// scene finishes loading, for smooth transitions between menus and levels. Uses unscaled time so
/// it still works while Time.timeScale is 0 (e.g. leaving a paused/won/lost screen).
/// </summary>
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    public float fadeDuration = 0.4f;

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
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        StartCoroutine(Fade(1f, 0f));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(1f, 0f));
    }

    public IEnumerator Fade(float from, float to)
    {
        if (overlay == null) yield break;

        float t = 0f;
        Color c = overlay.color;
        while (t < fadeDuration)
        {
            float alpha = Mathf.Lerp(from, to, t / fadeDuration);
            overlay.color = new Color(c.r, c.g, c.b, alpha);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        overlay.color = new Color(c.r, c.g, c.b, to);
    }
}

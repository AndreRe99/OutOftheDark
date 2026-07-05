using UnityEngine;

/// <summary>
/// Persistent audio singleton (survives scene loads): music/ambient/SFX volume (saved via
/// PlayerPrefs) and named one-shot hooks for every game event. AudioClip fields are intentionally
/// left unassigned - no audio files were available to generate. Every Play call is null-safe, so
/// the game stays fully functional and silent until real clips are dropped onto this component.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const string MusicVolumeKey = "audio_music_volume";
    private const string SfxVolumeKey = "audio_sfx_volume";

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip ambientLoop;

    [Header("SFX")]
    public AudioClip shotFired;
    public AudioClip reflection;
    public AudioClip teleport;
    public AudioClip switchClick;
    public AudioClip goalReached;
    public AudioClip levelComplete;
    public AudioClip levelFailed;
    public AudioClip menuClick;

    private AudioSource musicSource;
    private AudioSource ambientSource;
    private AudioSource sfxSource;

    public float MusicVolume { get; private set; } = 1f;
    public float SfxVolume { get; private set; } = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        sfxSource = gameObject.AddComponent<AudioSource>();

        MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        ApplyVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
        ApplyVolumes();
    }

    public void SetSfxVolume(float volume)
    {
        SfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        musicSource.volume = MusicVolume;
        ambientSource.volume = MusicVolume * 0.6f;
        sfxSource.volume = SfxVolume;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null || ambientSource.clip == clip) return;
        ambientSource.clip = clip;
        ambientSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, SfxVolume);
    }

    public void PlayShotFired() => PlaySfx(shotFired);
    public void PlayReflection() => PlaySfx(reflection);
    public void PlayTeleport() => PlaySfx(teleport);
    public void PlaySwitchClick() => PlaySfx(switchClick);
    public void PlayGoalReached() => PlaySfx(goalReached);
    public void PlayLevelComplete() => PlaySfx(levelComplete);
    public void PlayLevelFailed() => PlaySfx(levelFailed);
    public void PlayMenuClick() => PlaySfx(menuClick);
}

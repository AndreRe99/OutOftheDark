using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Subtle sine-wave pulsation of a Light2D's intensity, for a livelier "flickering light" look on
/// static lit objects (Lamp, AreaRevealer, ...). LightRay bakes its own pulsation directly instead
/// of using this, since it already owns its Light2D's intensity via distance attenuation.
/// </summary>
public class Pulsate : MonoBehaviour
{
    public float speed = 3f;
    [Range(0f, 1f)] public float amount = 0.15f;

    private Light2D light2D;
    private float baseIntensity;
    private float seed;

    void Awake()
    {
        light2D = GetComponentInChildren<Light2D>();
        baseIntensity = light2D != null ? light2D.intensity : 1f;
        seed = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        if (light2D == null) return;
        float wave = Mathf.Sin(Time.time * speed + seed);
        light2D.intensity = baseIntensity * (1f + wave * amount);
    }
}

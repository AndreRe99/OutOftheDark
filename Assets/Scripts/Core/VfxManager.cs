using UnityEngine;

/// <summary>
/// Persistent VFX singleton (survives scene loads): spawns short-lived particle bursts for common
/// ray events (impact spark, absorb, teleport, goal reached). Effect prefabs are optional - every
/// Spawn call is null-safe, so the game works fine even before any effect prefab is assigned.
/// </summary>
public class VfxManager : MonoBehaviour
{
    public static VfxManager Instance { get; private set; }

    public ParticleSystem sparkEffectPrefab;
    public ParticleSystem absorbEffectPrefab;
    public ParticleSystem teleportEffectPrefab;
    public ParticleSystem goalEffectPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SpawnSpark(Vector2 position, Color color) => Spawn(sparkEffectPrefab, position, color);

    public void SpawnAbsorb(Vector2 position) => Spawn(absorbEffectPrefab, position, Color.white);

    public void SpawnTeleport(Vector2 position) => Spawn(teleportEffectPrefab, position, Color.white);

    public void SpawnGoalBurst(Vector2 position) => Spawn(goalEffectPrefab, position, Color.green);

    private void Spawn(ParticleSystem prefab, Vector2 position, Color color)
    {
        if (prefab == null) return;

        ParticleSystem instance = Instantiate(prefab, position, Quaternion.identity);
        ParticleSystem.MainModule main = instance.main;
        main.startColor = color;

        float lifetime = main.duration + main.startLifetime.constantMax + 0.5f;
        Destroy(instance.gameObject, lifetime);
    }
}

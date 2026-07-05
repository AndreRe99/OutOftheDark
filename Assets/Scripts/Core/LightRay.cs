using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// A physically simulated light projectile (Kinematic Rigidbody2D). Moves in a straight line
/// until a LightInteractable redirects, splits, or absorbs it.
/// </summary>
public class LightRay : MonoBehaviour
{
    private const int MaxGeneration = 4;

    [Header("Movement")]
    public float speed = 5f;
    [SerializeField] private Vector2 direction = Vector2.right;

    [Header("Light Properties")]
    public Color color = Color.white;
    [Range(0f, 1f)] public float intensity = 1f;

    [Header("Failsafes")]
    public float maxLifetime = 10f;
    public float maxRange = 50f;

    [Header("Distance Attenuation")]
    [Tooltip("Constant/linear/quadratic terms of the point-light attenuation model (see OpticsMath.PointLightAttenuation).")]
    public float attenuationConstant = 1f;
    public float attenuationLinear = 0.02f;
    public float attenuationQuadratic = 0.002f;
    [Tooltip("Once intensity * attenuation drops below this, the ray has faded out and terminates.")]
    [Range(0f, 0.5f)] public float minVisibleIntensity = 0.04f;

    [Header("Splitting")]
    public int generation = 0;
    [HideInInspector] public GameObject sourcePrefab;

    [Header("Visuals")]
    [Tooltip("Optional child SpriteRenderer that gets the soft radial-gradient glow sprite assigned at runtime.")]
    public SpriteRenderer glowRenderer;
    public float glowScale = 3f;
    [Tooltip("Sine-wave pulsation speed/amount for a livelier, flickering light look.")]
    public float pulsateSpeed = 4f;
    [Range(0f, 0.5f)] public float pulsateAmount = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Light2D light2D;
    private TrailRenderer trail;
    private float traveledDistance;
    private float spawnTime;
    private float pulseSeed;
    private bool terminated;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = true;

        sr = GetComponent<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();
        trail = GetComponent<TrailRenderer>();
        pulseSeed = Random.Range(0f, Mathf.PI * 2f); // desync pulsation across simultaneous rays

        if (glowRenderer != null)
        {
            glowRenderer.sprite = RadialGradientTexture.GetGlowSprite();
            glowRenderer.transform.localScale = Vector3.one * glowScale;
        }

        if (trail != null && sr != null)
        {
            // Replaces Unity's magenta "no material" fallback with a soft, scene-lit gradient so the
            // trail reads as a glowing beam instead of a flat, hard-edged line.
            trail.material = RadialGradientTexture.GetTrailMaterial(sr.sharedMaterial);
        }
    }

    void Start()
    {
        spawnTime = Time.time;
        ApplyVisuals();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        rb.velocity = direction * speed;
        traveledDistance += speed * Time.fixedDeltaTime;

        // Distance attenuation (see OpticsMath.PointLightAttenuation): the ray's visible
        // brightness fades the further it travels, like a point light losing intensity over
        // distance. Purely visual/burnout, "intensity" itself (queried by other mechanics, e.g.
        // Prism splitting) stays the clean source value.
        float attenuation = OpticsMath.PointLightAttenuation(traveledDistance, attenuationConstant, attenuationLinear, attenuationQuadratic);
        ApplyVisuals(attenuation);

        bool burnedOut = intensity * attenuation < minVisibleIntensity;
        if (Time.time - spawnTime > maxLifetime || traveledDistance > maxRange || burnedOut)
        {
            Terminate();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AbsorbWand") || collision.name.Contains("AbsorbWand"))
        {
            Terminate();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("AbsorbWand") || collision.gameObject.name.Contains("AbsorbWand"))
        {
            Terminate();
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public Vector2 GetDirection() => direction;

    public void SetSpeed(float newSpeed) => speed = newSpeed;

    public void SetColor(Color newColor)
    {
        color = newColor;
        ApplyVisuals();
    }

    public Color GetColor() => color;

    public void SetIntensity(float newIntensity)
    {
        intensity = Mathf.Clamp01(newIntensity);
        ApplyVisuals();
    }

    public float GetIntensity() => intensity;

    private void ApplyVisuals(float attenuation = 1f)
    {
        // Subtle sine-wave pulsation for a livelier, flickering light look. pulseSeed desyncs
        // multiple simultaneous rays so they don't all pulse in lockstep.
        float pulse = 1f + Mathf.Sin(Time.time * pulsateSpeed + pulseSeed) * pulsateAmount;
        float visibleIntensity = intensity * attenuation * pulse;

        if (sr != null)
        {
            Color c = color;
            c.a = Mathf.Max(c.a, visibleIntensity);
            sr.color = c;
        }

        if (glowRenderer != null)
        {
            Color glowColor = color;
            glowColor.a = Mathf.Clamp01(visibleIntensity * 0.6f);
            glowRenderer.color = glowColor;
        }

        if (light2D != null)
        {
            light2D.color = color;
            light2D.intensity = visibleIntensity;
        }

        if (trail != null)
        {
            trail.startColor = new Color(color.r, color.g, color.b, Mathf.Clamp01(visibleIntensity));
            trail.endColor = new Color(color.r, color.g, color.b, 0f);
        }
    }

    /// <summary>
    /// Ends this ray's life. Central exit point so future VFX/pooling hooks in one place.
    /// </summary>
    public void Terminate()
    {
        if (terminated) return;
        terminated = true;
        Destroy(gameObject);
    }

    /// <summary>
    /// Spawns a child ray (e.g. from a Prism or splitter) using the same visual prefab.
    /// Returns null once the recursion cap is reached.
    /// </summary>
    public LightRay SpawnChild(Vector2 childDirection, Color childColor, float childIntensity, Vector3? spawnOffset = null)
    {
        if (generation >= MaxGeneration || sourcePrefab == null)
        {
            return null;
        }

        Vector3 spawnPosition = transform.position + (spawnOffset ?? Vector3.zero);
        GameObject instance = Instantiate(sourcePrefab, spawnPosition, Quaternion.identity);
        LightRay child = instance.GetComponent<LightRay>();
        if (child == null)
        {
            child = instance.AddComponent<LightRay>();
        }

        child.sourcePrefab = sourcePrefab;
        child.generation = generation + 1;
        child.SetSpeed(speed);
        child.SetDirection(childDirection);
        child.SetColor(childColor);
        child.SetIntensity(childIntensity);

        return child;
    }

    /// <summary>
    /// Factory method used by LightSource to spawn the initial ray fired by the player.
    /// </summary>
    public static LightRay CreateAndInitialize(GameObject prefab, Vector3 position, float speed, Vector2 direction,
        Color? color = null, float? intensity = null)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        LightRay ray = instance.GetComponent<LightRay>();
        if (ray == null)
        {
            ray = instance.AddComponent<LightRay>();
        }

        ray.sourcePrefab = prefab;
        ray.generation = 0;
        ray.SetSpeed(speed);
        ray.SetDirection(direction);
        if (color.HasValue) ray.SetColor(color.Value);
        if (intensity.HasValue) ray.SetIntensity(intensity.Value);

        return ray;
    }
}

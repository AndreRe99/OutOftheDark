using UnityEngine;

/// <summary>
/// What an EnergyField does to a ray that enters it.
/// </summary>
public enum EnergyFieldMode
{
    Block,
    SlowDown,
    SpeedUp,
    ChangeColor,
    ChangeIntensity
}

/// <summary>
/// Region that affects a ray's speed, color, or intensity when entered - or blocks it outright.
/// Can be toggled on/off at runtime (e.g. via a Switch); while disabled it lets everything through
/// unchanged.
/// </summary>
public class EnergyField : LightInteractable
{
    public EnergyFieldMode mode = EnergyFieldMode.SlowDown;
    public bool isEnabled = true;

    [Tooltip("Used by SlowDown (<1) and SpeedUp (>1): new speed = ray.speed * speedMultiplier.")]
    public float speedMultiplier = 0.5f;
    public Color fieldColor = Color.cyan;
    public float intensityMultiplier = 0.5f;

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = enabled ? 0.5f : 0.1f;
            sr.color = c;
        }
    }

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (!isEnabled)
        {
            return InteractionResult.PassThrough();
        }

        switch (mode)
        {
            case EnergyFieldMode.Block:
                return InteractionResult.Absorb();

            case EnergyFieldMode.SlowDown:
            case EnergyFieldMode.SpeedUp:
                ray.SetSpeed(ray.speed * speedMultiplier);
                return InteractionResult.PassThrough();

            case EnergyFieldMode.ChangeColor:
                return InteractionResult.Redirect(ray.GetDirection(), fieldColor);

            case EnergyFieldMode.ChangeIntensity:
                return InteractionResult.Redirect(ray.GetDirection(), newIntensity: Mathf.Clamp01(ray.GetIntensity() * intensityMultiplier));

            default:
                return InteractionResult.PassThrough();
        }
    }
}

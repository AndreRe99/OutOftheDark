using UnityEngine;

/// <summary>
/// Permanently boosts a ray's speed the moment it passes through - a "nitro boost" for light.
/// Also extends maxRange so the boosted ray can actually make use of the extra speed before its
/// distance-attenuation/lifetime failsafe would otherwise end it early.
/// </summary>
public class LightAccelerator : LightInteractable
{
    public float speedMultiplier = 1.5f;
    public float extraRange = 10f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        ray.SetSpeed(ray.speed * speedMultiplier);
        ray.maxRange += extraRange;
        return InteractionResult.PassThrough();
    }
}

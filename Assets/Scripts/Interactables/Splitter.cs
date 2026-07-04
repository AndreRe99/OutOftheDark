using UnityEngine;

/// <summary>
/// Splits an incoming ray into several independent rays fanned around the original direction,
/// each keeping the original color and getting its own physics/collisions. Unlike Prism (which
/// splits light by wavelength into red/green/blue), a Splitter always keeps the same color.
/// </summary>
public class Splitter : LightInteractable
{
    public int rayCount = 3;
    public float spreadAngle = 40f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        var children = BuildFan(ray.GetDirection(), ray.GetColor(), ray.GetIntensity(), rayCount, spreadAngle);
        return InteractionResult.Split(children);
    }
}

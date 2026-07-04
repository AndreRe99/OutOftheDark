using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Splits a white ray into red/green/blue child rays using Snell's law with a slightly different
/// refractive index per color (chromatic dispersion) - exactly why a real prism fans white light
/// into a spectrum: n_blue > n_green > n_red means blue bends the most. Green is treated as the
/// prism's reference wavelength and passes straight through undeviated; red/blue are genuinely
/// refracted around it via OpticsMath.Refract. Already-colored rays (generation > 0, e.g. re-
/// entering another prism) pass through unchanged - they have already been split once.
/// </summary>
public class Prism : LightInteractable
{
    public OpticalMaterial material;
    [Range(0f, 1f)] public float colorTolerance = 0.1f;

    private const float AirRefractiveIndex = 1f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        Vector2 incoming = ray.GetDirection();

        if (ray.generation > 0 || !IsApproximatelyWhite(ray.GetColor()))
        {
            return InteractionResult.Redirect(incoming, ray.GetColor());
        }

        float baseIndex = material != null ? material.refractiveIndex : 1.5f;
        Vector3 dispersion = material != null ? material.dispersionOffsetRGB : new Vector3(-0.15f, 0f, 0.15f);
        float childIntensity = ray.GetIntensity() / 3f;

        var children = new List<ChildRaySpec>
        {
            new ChildRaySpec(RefractOrKeep(incoming, hitNormal, baseIndex + dispersion.x), Color.red, childIntensity),
            new ChildRaySpec(incoming, Color.green, childIntensity),
            new ChildRaySpec(RefractOrKeep(incoming, hitNormal, baseIndex + dispersion.z), Color.blue, childIntensity),
        };

        return InteractionResult.Split(children);
    }

    /// <summary>Snell's law refraction (OpticsMath.Refract) from air (n=1) into n2; falls back to
    /// the unbent incoming direction on total internal reflection.</summary>
    private Vector2 RefractOrKeep(Vector2 incoming, Vector2 normal, float n2)
    {
        return OpticsMath.Refract(incoming, normal, AirRefractiveIndex, n2, out Vector2 refracted)
            ? refracted
            : incoming;
    }

    private bool IsApproximatelyWhite(Color c)
    {
        float dist = Mathf.Sqrt(Mathf.Pow(c.r - 1f, 2) + Mathf.Pow(c.g - 1f, 2) + Mathf.Pow(c.b - 1f, 2));
        return dist <= colorTolerance * Mathf.Sqrt(3f);
    }
}

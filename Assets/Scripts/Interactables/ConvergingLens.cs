using UnityEngine;

/// <summary>
/// Two modes: if useIdealizedFocus is true (default), the ray is aimed toward focalPoint/
/// focalDistance - an idealized thin lens, useful when a level needs a precisely aimed lens.
/// Otherwise it uses a physically based single refraction interface via Snell's law
/// (OpticsMath.Refract) and the lens' OpticalMaterial: entering a denser medium bends the ray
/// toward the surface normal, which is the same reason a real convex lens surface bends light
/// inward.
/// </summary>
public class ConvergingLens : LightInteractable
{
    [Tooltip("Idealized thin-lens aiming (for precise level design) vs. real Snell's law refraction.")]
    public bool useIdealizedFocus = true;
    public Transform focalPoint;
    public float focalDistance = 3f;
    [Range(0f, 1f)] public float bendStrength = 0.6f;
    public OpticalMaterial material;

    private const float AirRefractiveIndex = 1f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (useIdealizedFocus)
        {
            Vector2 target = focalPoint != null
                ? (Vector2)focalPoint.position
                : (Vector2)transform.position + hitNormal * focalDistance;

            Vector2 toFocus = (target - hitPoint).normalized;
            Vector2 aimed = Vector2.Lerp(ray.GetDirection(), toFocus, bendStrength).normalized;
            return InteractionResult.Redirect(aimed);
        }

        float n2 = material != null ? material.refractiveIndex : 1.5f;
        Vector2 refracted = OpticsMath.Refract(ray.GetDirection(), hitNormal, AirRefractiveIndex, n2, out Vector2 result)
            ? result
            : OpticsMath.Reflect(ray.GetDirection(), hitNormal); // total internal reflection

        return InteractionResult.Redirect(refracted);
    }
}

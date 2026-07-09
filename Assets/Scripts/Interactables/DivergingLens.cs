using UnityEngine;

/// <summary>
/// Physically based diverging lens: refracts via Snell's law (OpticsMath.Refract) using an
/// inverted local normal to emulate a concave entry surface - the opposite curvature of
/// ConvergingLens - which bends off-axis rays away from each other instead of toward each other.
/// </summary>
public class DivergingLens : LightInteractable
{
    public OpticalMaterial material;

    private const float AirRefractiveIndex = 1f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        float n2 = material != null ? material.refractiveIndex : 1.5f;
        Vector2 concaveNormal = -hitNormal;

        Vector2 refracted = OpticsMath.Refract(ray.GetDirection(), concaveNormal, AirRefractiveIndex, n2, out Vector2 result)
            ? result
            : OpticsMath.Reflect(ray.GetDirection(), hitNormal);

        Debug.Log("DivergingLens: hitPoint = " + hitPoint + ", concaveNormal = " + concaveNormal + ", refracted = " + refracted);
        return InteractionResult.Redirect(refracted);
    }
}

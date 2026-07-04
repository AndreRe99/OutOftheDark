using UnityEngine;

/// <summary>
/// Reflects a ray only if its color matches reflectColor within tolerance; any other color passes
/// straight through untouched, as if the mirror weren't there.
/// </summary>
public class ColorSelectiveMirror : LightInteractable
{
    public Color reflectColor = Color.red;
    [Range(0f, 1f)] public float tolerance = 0.15f;
    [Range(0f, 1f)] public float reflectivity = 1f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (!ColorMatches(ray.GetColor(), reflectColor, tolerance))
        {
            return InteractionResult.PassThrough();
        }

        Vector2 reflected = OpticsMath.Reflect(ray.GetDirection(), hitNormal).normalized;
        return InteractionResult.Redirect(reflected, newIntensity: ray.GetIntensity() * reflectivity);
    }

    private bool ColorMatches(Color a, Color b, float tol)
    {
        float dist = Mathf.Sqrt(Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g - b.g, 2) + Mathf.Pow(a.b - b.b, 2));
        return dist <= tol * Mathf.Sqrt(3f);
    }
}

using UnityEngine;

/// <summary>
/// Reflects an incoming ray around its surface normal (transform.up). Optional glossiness adds a
/// small random scatter angle per hit; reflectivity attenuates intensity on every bounce.
/// </summary>
public class Mirror : LightInteractable
{
    [Range(0f, 1f)] public float reflectivity = 1f;
    [Range(0f, 1f)] public float glossiness = 0f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        Vector2 incoming = ray.GetDirection();

        // Reflection vector: r = d - 2(d.n)n (OpticsMath.Reflect), i.e. angle of incidence = angle
        // of reflection around the mirror's surface normal.
        Vector2 reflected = OpticsMath.Reflect(incoming, hitNormal).normalized;

        if (glossiness > 0f)
        {
            float maxAngle = Mathf.Lerp(0f, 30f, glossiness);
            float angle = Random.Range(-maxAngle, maxAngle) * Mathf.Deg2Rad;
            reflected = RotateVector(reflected, angle);
        }

        // Fresnel-Schlick: real reflective surfaces reflect more strongly at grazing angles than
        // head-on. We fold a small amount of that into reflectivity so shallow hits lose a little
        // less intensity per bounce than near-normal ones.
        float cosTheta = Mathf.Clamp01(Mathf.Abs(Vector2.Dot(incoming.normalized, hitNormal.normalized)));
        float fresnel = OpticsMath.FresnelSchlick(cosTheta, 1f, 1.5f);
        float effectiveReflectivity = Mathf.Clamp01(reflectivity + (1f - reflectivity) * fresnel);

        return InteractionResult.Redirect(reflected, newIntensity: ray.GetIntensity() * effectiveReflectivity);
    }
}

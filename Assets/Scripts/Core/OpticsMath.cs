using UnityEngine;

/// <summary>
/// Shared, textbook optics/lighting formulas used across the interactable mechanics. Kept in one
/// place so every mechanic cites the same, documented implementation instead of re-deriving it.
/// </summary>
public static class OpticsMath
{
    /// <summary>
    /// Reflection vector: r = d - 2(d.n)n (angle of incidence = angle of reflection).
    /// n must be normalized. Used by Mirror.
    /// </summary>
    public static Vector2 Reflect(Vector2 incident, Vector2 normal)
    {
        return incident - 2f * Vector2.Dot(incident, normal) * normal;
    }

    /// <summary>
    /// Snell's law in vector form (as used in ray tracing / GLSL's refract()):
    /// eta = n1/n2, cosI = -n.d, sin2T = eta^2 * (1 - cosI^2).
    /// If sin2T > 1 the ray undergoes total internal reflection (no real refraction angle exists)
    /// and this returns false. Otherwise:
    /// t = eta*d + (eta*cosI - sqrt(1-sin2T)) * n
    /// n1/n2 are the refractive indices of the medium the ray leaves/enters (see OpticalMaterial).
    /// Used by Prism (per-color dispersion) and the lenses.
    /// </summary>
    public static bool Refract(Vector2 incident, Vector2 normal, float n1, float n2, out Vector2 refracted)
    {
        Vector2 d = incident.normalized;
        Vector2 n = normal.normalized;

        float cosI = -Vector2.Dot(n, d);
        if (cosI < 0f)
        {
            // Normal points away from the incoming ray; flip it so the formula below holds.
            n = -n;
            cosI = -cosI;
        }

        float eta = n1 / n2;
        float sin2T = eta * eta * (1f - cosI * cosI);

        if (sin2T > 1f)
        {
            refracted = default;
            return false; // total internal reflection
        }

        float cosT = Mathf.Sqrt(1f - sin2T);
        refracted = (eta * d + (eta * cosI - cosT) * n).normalized;
        return true;
    }

    /// <summary>
    /// Schlick's approximation of the Fresnel reflectance: how much light reflects vs. refracts at
    /// a boundary, depending on the incidence angle. R0 is the reflectance at normal incidence
    /// (0 degrees); reflectance rises towards 1 at grazing angles.
    /// R0 = ((n1-n2)/(n1+n2))^2 ,  R(theta) = R0 + (1-R0)(1-cosTheta)^5
    /// </summary>
    public static float FresnelSchlick(float cosTheta, float n1, float n2)
    {
        float r0 = (n1 - n2) / (n1 + n2);
        r0 *= r0;
        float x = Mathf.Clamp01(1f - cosTheta);
        return r0 + (1f - r0) * (x * x * x * x * x);
    }

    /// <summary>
    /// Lambert's cosine law for diffuse reflectance: brightness is proportional to the cosine of
    /// the angle between the surface normal and the direction to the light, clamped to zero for
    /// surfaces facing away. I_diffuse = max(0, N . L).
    /// </summary>
    public static float LambertDiffuse(Vector2 normal, Vector2 lightDir)
    {
        return Mathf.Max(0f, Vector2.Dot(normal.normalized, lightDir.normalized));
    }

    /// <summary>
    /// Blinn-Phong specular term using the half-vector H = normalize(L + V) instead of the mirror
    /// reflection vector (cheaper, avoids a Reflect() call, visually near-identical to Phong).
    /// I_spec = max(0, N . H) ^ shininess.
    /// </summary>
    public static float BlinnPhongSpecular(Vector2 normal, Vector2 lightDir, Vector2 viewDir, float shininess)
    {
        Vector2 half = (lightDir.normalized + viewDir.normalized).normalized;
        float nDotH = Mathf.Max(0f, Vector2.Dot(normal.normalized, half));
        return Mathf.Pow(nDotH, Mathf.Max(1f, shininess));
    }

    /// <summary>
    /// Beer-Lambert law for absorption through a medium: transmittance falls off exponentially
    /// with distance/thickness and the medium's absorption coefficient.
    /// T = e^(-absorptionCoefficient * distance)
    /// </summary>
    public static float BeerLambertTransmittance(float absorptionCoefficient, float distance)
    {
        return Mathf.Exp(-Mathf.Max(0f, absorptionCoefficient) * Mathf.Max(0f, distance));
    }

    /// <summary>
    /// Classic point-light distance attenuation (constant + linear + quadratic terms), the model
    /// asked for directly in the "Positionslicht" slides: att(d) = 1 / (c0 + c1*d + c2*d^2).
    /// Clamped to [0,1] since attenuation must never brighten a ray.
    /// </summary>
    public static float PointLightAttenuation(float distance, float constant, float linear, float quadratic)
    {
        float denom = constant + linear * distance + quadratic * distance * distance;
        return Mathf.Clamp01(1f / Mathf.Max(0.0001f, denom));
    }
}

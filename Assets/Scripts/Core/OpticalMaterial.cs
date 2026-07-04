using UnityEngine;

/// <summary>
/// Data-driven optical properties for a material (glass, water, diamond, ...), so Prism/lenses can
/// reference "which material" instead of hardcoding a refractive index per script.
/// </summary>
[CreateAssetMenu(fileName = "OpticalMaterial", menuName = "OutOfTheDark/Optical Material")]
public class OpticalMaterial : ScriptableObject
{
    [Tooltip("n in Snell's law. Vacuum/air = 1.0, water ~1.33, glass ~1.5, diamond ~2.42.")]
    public float refractiveIndex = 1.5f;

    [Tooltip("Beer-Lambert absorption coefficient (higher = absorbs light faster with distance).")]
    public float absorptionCoefficient = 0.05f;

    [Tooltip("Blinn-Phong shininess exponent for the specular highlight (higher = tighter/shinier highlight).")]
    public float shininess = 32f;

    [Tooltip("Per-channel refractive index offset used for dispersion (Prism): red/green/blue each bend a little differently.")]
    public Vector3 dispersionOffsetRGB = new Vector3(-0.01f, 0f, 0.01f);
}

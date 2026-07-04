using UnityEngine;

/// <summary>
/// Physically motivated color filter: transmission multiplies the ray's color channel-wise by the
/// filter's own color (a pure red filter zeroes out green/blue - the standard model for how color
/// gels/filters work), and Beer-Lambert's law (OpticsMath.BeerLambertTransmittance) dims whatever
/// survives based on the filter's absorption coefficient and thickness. If too little light (or
/// the wrong color entirely) gets through, the ray is treated as fully absorbed.
/// </summary>
public class ColorFilter : LightInteractable
{
    public Color filterColor = Color.red;
    [Tooltip("Beer-Lambert absorption coefficient of the filter medium.")]
    public float absorptionCoefficient = 0.3f;
    [Tooltip("Effective thickness used in the Beer-Lambert calculation.")]
    public float thickness = 1f;
    [Range(0f, 0.5f)] public float minTransmittedIntensity = 0.05f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        Color incoming = ray.GetColor();

        Color transmitted = new Color(
            incoming.r * filterColor.r,
            incoming.g * filterColor.g,
            incoming.b * filterColor.b,
            1f);

        float transmittance = OpticsMath.BeerLambertTransmittance(absorptionCoefficient, thickness);
        float outIntensity = ray.GetIntensity() * transmittance;

        float survivingChannel = Mathf.Max(transmitted.r, transmitted.g, transmitted.b);
        if (survivingChannel < minTransmittedIntensity || outIntensity < minTransmittedIntensity)
        {
            return InteractionResult.Absorb();
        }

        return InteractionResult.Redirect(ray.GetDirection(), transmitted, outIntensity);
    }
}

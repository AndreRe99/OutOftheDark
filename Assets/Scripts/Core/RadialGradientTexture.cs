using UnityEngine;

/// <summary>
/// Generates (once, cached, shared by every LightRay instance) a soft radial-gradient sprite used
/// for the light ball's glow halo. Alpha falls off from opaque center to fully transparent edge -
/// on this game's pure-black background that reads visually the same as true additive blending,
/// without needing a custom additive shader whose cross-render-pipeline behavior we can't
/// interactively preview here. Cached so generating many simultaneous rays stays cheap.
/// </summary>
public static class RadialGradientTexture
{
    private static Sprite cachedGlow;
    private static Texture2D cachedTrailGradient;
    private static Material cachedTrailMaterial;

    public static Sprite GetGlowSprite()
    {
        if (cachedGlow != null) return cachedGlow;

        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxDist = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / maxDist;
                float alpha = Mathf.Clamp01(1f - dist);
                alpha *= alpha; // steeper falloff towards the edge, closer to a real light falloff
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        tex.Apply();

        cachedGlow = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return cachedGlow;
    }

    /// <summary>
    /// A 1px-wide, vertically soft-edged gradient (opaque center row, fading to transparent top/bottom).
    /// Stretched across a TrailRenderer's width by its Stretch texture mode, this replaces a flat,
    /// hard-edged trail strip with a soft glowing beam cross-section - same falloff trick as
    /// GetGlowSprite, just 1D so it tiles cleanly along the trail's length with no seams.
    /// </summary>
    public static Texture2D GetTrailGradientTexture()
    {
        if (cachedTrailGradient != null) return cachedTrailGradient;

        const int size = 64;
        var tex = new Texture2D(1, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        for (int y = 0; y < size; y++)
        {
            float v = (y + 0.5f) / size;
            float distFromCenter = Mathf.Abs(v - 0.5f) * 2f;
            float alpha = Mathf.Clamp01(1f - distFromCenter);
            alpha *= alpha;
            tex.SetPixel(0, y, new Color(1f, 1f, 1f, alpha));
        }
        tex.Apply();

        cachedTrailGradient = tex;
        return cachedTrailGradient;
    }

    /// <summary>
    /// Clones the given (already-working, scene-lit) sprite material and swaps in the soft trail
    /// gradient as its main texture, so the trail renders with the same shader/lighting as the rest
    /// of the ray instead of Unity's magenta "no material" fallback.
    /// </summary>
    public static Material GetTrailMaterial(Material template)
    {
        if (cachedTrailMaterial != null) return cachedTrailMaterial;
        if (template == null) return null;

        cachedTrailMaterial = new Material(template) { mainTexture = GetTrailGradientTexture() };
        return cachedTrailMaterial;
    }
}

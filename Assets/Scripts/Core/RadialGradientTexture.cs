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
}

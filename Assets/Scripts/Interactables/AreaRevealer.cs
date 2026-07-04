using UnityEngine;

/// <summary>
/// Reveals the fog-of-war permanently in a radius around itself the first time a LightRay hits
/// it, then lets the ray continue unaffected. Backs light crystals, glow stones, and light towers
/// alike - same behavior, just a different revealRadius per prefab.
/// </summary>
public class AreaRevealer : LightInteractable
{
    public float revealRadius = 3f;
    public float fadeDuration = 0.6f;
    public Color litColor = new Color(1f, 0.95f, 0.6f, 1f);

    private bool activated;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (!activated)
        {
            activated = true;
            FogOfWarManager.RevealCircle(transform.position, revealRadius, fadeDuration);

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = litColor;
        }

        return InteractionResult.PassThrough();
    }
}

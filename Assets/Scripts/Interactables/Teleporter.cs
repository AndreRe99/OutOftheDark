using UnityEngine;

/// <summary>
/// Paired teleporters: a ray entering one instantly exits at the linked partner, keeping its
/// direction and speed. Can be toggled on/off at runtime (e.g. via a Switch).
/// </summary>
public class Teleporter : LightInteractable
{
    public Teleporter linkedTeleporter;
    public bool isEnabled = true;
    [Tooltip("How far past the linked teleporter the ray re-emerges, so it doesn't immediately re-trigger it.")]
    public float exitOffset = 0.8f;

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = enabled ? 1f : 0.3f;
            sr.color = c;
        }
    }

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (!isEnabled || linkedTeleporter == null || !linkedTeleporter.isEnabled)
        {
            return InteractionResult.PassThrough();
        }

        Vector2 direction = ray.GetDirection();
        Vector2 exitPosition = (Vector2)linkedTeleporter.transform.position + direction * exitOffset;
        return InteractionResult.Teleport(exitPosition, direction);
    }
}

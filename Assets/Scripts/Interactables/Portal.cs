using UnityEngine;

/// <summary>
/// Paired like a Teleporter, but visually distinct and can optionally override the ray's exit
/// direction instead of always preserving it (e.g. a portal that always sends light due "north").
/// </summary>
public class Portal : LightInteractable
{
    public Portal linkedPortal;
    public bool overrideExitDirection = false;
    [Tooltip("Used as the exit direction when overrideExitDirection is true.")]
    public Vector2 exitDirection = Vector2.up;
    public float exitOffset = 0.8f;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (linkedPortal == null)
        {
            return InteractionResult.PassThrough();
        }

        Vector2 direction = overrideExitDirection ? exitDirection.normalized : ray.GetDirection();
        Vector2 exitPosition = (Vector2)linkedPortal.transform.position + direction * exitOffset;
        return InteractionResult.Teleport(exitPosition, direction);
    }
}

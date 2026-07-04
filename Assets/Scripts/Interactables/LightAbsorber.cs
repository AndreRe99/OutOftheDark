using UnityEngine;

/// <summary>
/// Simply destroys any ray that touches it. Used for level boundary walls and dead-end traps.
/// </summary>
public class LightAbsorber : LightInteractable
{
    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        return InteractionResult.Absorb();
    }
}

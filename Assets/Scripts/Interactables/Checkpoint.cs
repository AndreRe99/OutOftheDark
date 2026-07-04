using UnityEngine;

/// <summary>
/// Records level progress when a ray reaches it: a later restart (after running out of light
/// balls) respawns from here via CheckpointManager instead of reloading the whole level.
/// </summary>
public class Checkpoint : LightInteractable
{
    [Tooltip("If true, ammo is fully reset on respawn; if false, whatever was remaining at this checkpoint is restored.")]
    public bool resetAmmoOnRespawn = false;
    public Color activatedColor = Color.yellow;

    private bool activated;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (!activated)
        {
            activated = true;

            CheckpointManager manager = FindObjectOfType<CheckpointManager>();
            if (manager != null)
            {
                manager.SetCheckpoint(transform.position, resetAmmoOnRespawn);
            }

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = activatedColor;
        }

        return InteractionResult.PassThrough();
    }
}

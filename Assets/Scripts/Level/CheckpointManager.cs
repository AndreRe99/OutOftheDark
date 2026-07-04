using UnityEngine;

/// <summary>
/// Tracks the last Checkpoint reached in this level. LevelFailedController consults this on
/// restart: if a checkpoint was reached, the LightSource respawns there instead of a full scene
/// reload, and ammo is either kept (whatever was left at the checkpoint) or fully reset, per the
/// checkpoint's own rule.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    public bool HasCheckpoint { get; private set; }

    private Vector3 checkpointPosition;
    private bool resetAmmoOnRespawn;
    private int ammoAtCheckpoint;

    public void SetCheckpoint(Vector3 position, bool resetAmmo)
    {
        HasCheckpoint = true;
        checkpointPosition = position;
        resetAmmoOnRespawn = resetAmmo;

        ShotBudget budget = FindObjectOfType<ShotBudget>();
        ammoAtCheckpoint = budget != null ? budget.Remaining : 0;
    }

    public void RespawnAtCheckpoint()
    {
        if (!HasCheckpoint) return;

        LightSource source = FindObjectOfType<LightSource>();
        if (source != null)
        {
            source.transform.position = checkpointPosition;
        }

        ShotBudget budget = FindObjectOfType<ShotBudget>();
        if (budget != null)
        {
            int restoreTo = resetAmmoOnRespawn ? budget.maxShots : ammoAtCheckpoint;
            budget.RestoreRemaining(restoreTo);
        }
    }
}

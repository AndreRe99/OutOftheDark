using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Caps how many LightRays the player may fire in a level. Fires OnShotsChanged for the HUD after
/// every shot, and shows the fail screen once the budget is exhausted, the goal wasn't reached,
/// and every in-flight ray has come to rest (terminated/absorbed/reached the goal).
/// </summary>
public class ShotBudget : MonoBehaviour
{
    public int maxShots = 5;

    public int Remaining { get; private set; }
    public int Used { get; private set; }

    public event Action<int, int> OnShotsChanged; // (remaining, max)

    private bool levelWon;
    private bool failCheckRunning;

    void Awake()
    {
        Remaining = maxShots;
        Used = 0;
    }

    void Start()
    {
        OnShotsChanged?.Invoke(Remaining, maxShots);
    }

    public bool TryConsumeShot()
    {
        if (Remaining <= 0) return false;

        Remaining--;
        Used++;
        OnShotsChanged?.Invoke(Remaining, maxShots);

        if (Remaining == 0 && !failCheckRunning)
        {
            failCheckRunning = true;
            StartCoroutine(CheckForFailAfterLastShot());
        }

        return true;
    }

    public void NotifyLevelWon()
    {
        levelWon = true;
    }

    /// <summary>Used by CheckpointManager to restore ammo on a checkpoint respawn (either back to
    /// the amount left at the checkpoint, or a full reset, depending on the level's rule).</summary>
    public void RestoreRemaining(int amount)
    {
        Remaining = Mathf.Clamp(amount, 0, maxShots);
        failCheckRunning = false;
        OnShotsChanged?.Invoke(Remaining, maxShots);
    }

    private IEnumerator CheckForFailAfterLastShot()
    {
        yield return null; // let the just-fired ray spawn first

        while (FindObjectsOfType<LightRay>().Length > 0)
        {
            yield return null;
        }

        if (!levelWon)
        {
            LevelFailedController failUI = FindObjectOfType<LevelFailedController>();
            if (failUI != null) failUI.Show();
        }
    }
}

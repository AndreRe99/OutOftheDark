using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activated when a LightRay hits it; lets the ray continue unaffected and opens any linked Doors.
/// In latch mode (default) it stays activated once triggered; in toggle mode each hit flips it.
/// </summary>
public class Switch : LightInteractable
{
    public bool toggleMode = false;
    public Color inactiveColor = new Color(0.5f, 0.1f, 0.1f, 1f);
    public Color activeColor = new Color(0.2f, 1f, 0.3f, 1f);
    public List<Door> linkedDoors = new List<Door>();
    public List<HiddenObject> revealTargets = new List<HiddenObject>();
    public List<Teleporter> linkedTeleporters = new List<Teleporter>();
    public List<EnergyField> linkedEnergyFields = new List<EnergyField>();
    public List<MovingObject> linkedMovers = new List<MovingObject>();

    public bool IsActivated { get; private set; }

    void Start()
    {
        ApplyVisual();
    }

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (toggleMode)
        {
            SetActivated(!IsActivated);
        }
        else if (!IsActivated)
        {
            SetActivated(true);
        }

        return InteractionResult.PassThrough();
    }

    private void SetActivated(bool activated)
    {
        IsActivated = activated;
        ApplyVisual();
        AudioManager.Instance?.PlaySwitchClick();

        foreach (Door door in linkedDoors)
        {
            if (door != null) door.SetOpen(activated);
        }

        if (activated)
        {
            foreach (HiddenObject hidden in revealTargets)
            {
                if (hidden != null) hidden.Reveal();
            }
        }

        foreach (Teleporter teleporter in linkedTeleporters)
        {
            if (teleporter != null) teleporter.SetEnabled(activated);
        }

        foreach (EnergyField field in linkedEnergyFields)
        {
            if (field != null) field.SetEnabled(activated);
        }

        foreach (MovingObject mover in linkedMovers)
        {
            if (mover != null) mover.SetMoving(activated);
        }
    }

    private void ApplyVisual()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = IsActivated ? activeColor : inactiveColor;
    }
}

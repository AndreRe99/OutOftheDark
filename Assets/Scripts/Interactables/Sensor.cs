using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activated when a LightRay hits it: turns on every linked Lamp and lets the ray continue.
/// </summary>
public class Sensor : LightInteractable
{
    public List<Lamp> linkedLamps = new List<Lamp>();

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        foreach (Lamp lamp in linkedLamps)
        {
            if (lamp != null) lamp.Activate();
        }

        return InteractionResult.PassThrough();
    }
}

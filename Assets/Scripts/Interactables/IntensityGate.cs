using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Like a Switch, but only activates if the ray's intensity is at or above minIntensity - a dim
/// ray (e.g. attenuated by distance or dimmed by a filter) won't trigger it.
/// </summary>
public class IntensityGate : LightInteractable
{
    [Range(0f, 1f)] public float minIntensity = 0.5f;
    public List<Door> linkedDoors = new List<Door>();

    public bool IsActivated { get; private set; }

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        if (!IsActivated && ray.GetIntensity() >= minIntensity)
        {
            IsActivated = true;
            foreach (Door door in linkedDoors)
            {
                if (door != null) door.SetOpen(true);
            }

            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.white;
        }

        return InteractionResult.PassThrough();
    }
}

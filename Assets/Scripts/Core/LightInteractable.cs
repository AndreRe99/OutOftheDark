using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for every object that reacts to a LightRay hitting it (mirrors, lenses, prisms,
/// filters, absorbers, ...). Centralizes the re-trigger debounce and nudge logic that used to be
/// duplicated per mechanic, and dispatches to the concrete behavior via ProcessRay.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class LightInteractable : MonoBehaviour
{
    [Header("Debounce")]
    [SerializeField] protected float ignoreDuration = 0.05f;
    [SerializeField] protected float nudgeDistance = 0.05f;

    private readonly Dictionary<int, float> recentHits = new Dictionary<int, float>();

    protected virtual void Update()
    {
        PruneExpiredHits();
    }

    private void PruneExpiredHits()
    {
        if (recentHits.Count == 0) return;

        List<int> expired = null;
        float now = Time.time;
        foreach (var kvp in recentHits)
        {
            if (now - kvp.Value > ignoreDuration)
            {
                expired ??= new List<int>();
                expired.Add(kvp.Key);
            }
        }

        if (expired != null)
        {
            foreach (int key in expired) recentHits.Remove(key);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LightRay ray = collision.GetComponent<LightRay>();
        if (ray == null) return;

        int id = ray.GetInstanceID();
        if (recentHits.ContainsKey(id)) return;

        Vector2 hitPoint = collision.ClosestPoint(transform.position);
        Vector2 hitNormal = transform.up.normalized;

        InteractionResult result = ProcessRay(ray, hitPoint, hitNormal);
        ApplyResult(ray, result);

        recentHits[id] = Time.time;
    }

    private void ApplyResult(LightRay ray, InteractionResult result)
    {
        switch (result.Action)
        {
            case RayAction.Redirect:
                ray.SetDirection(result.NewDirection);
                ray.transform.position += (Vector3)(result.NewDirection.normalized * nudgeDistance);
                if (result.NewColor.HasValue) ray.SetColor(result.NewColor.Value);
                if (result.NewIntensity.HasValue) ray.SetIntensity(result.NewIntensity.Value);
                break;

            case RayAction.Split:
                if (result.ChildRays != null)
                {
                    foreach (ChildRaySpec child in result.ChildRays)
                    {
                        ray.SpawnChild(child.Direction, child.Color, child.Intensity,
                            (Vector3)(child.Direction.normalized * nudgeDistance));
                    }
                }
                ray.Terminate();
                break;

            case RayAction.Absorb:
                ray.Terminate();
                break;

            case RayAction.PassThrough:
                break;

            case RayAction.Teleport:
                ray.transform.position = result.TargetPosition;
                ray.SetDirection(result.NewDirection);
                break;
        }
    }

    /// <summary>
    /// Decide what happens to a ray that just entered this object's trigger collider.
    /// </summary>
    protected abstract InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal);

    /// <summary>
    /// Rotates a 2D vector by the given angle (radians). Shared helper for optics mechanics.
    /// </summary>
    protected static Vector2 RotateVector(Vector2 v, float angleRad)
    {
        float cos = Mathf.Cos(angleRad);
        float sin = Mathf.Sin(angleRad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }

    /// <summary>
    /// Builds `count` child ray specs fanned evenly across spreadAngle degrees around `direction`,
    /// each getting an equal share of totalIntensity. Shared by Splitter and Crystal (MultiRay mode).
    /// </summary>
    protected static List<ChildRaySpec> BuildFan(Vector2 direction, Color color, float totalIntensity, int count, float spreadAngle)
    {
        var children = new List<ChildRaySpec>();
        float childIntensity = totalIntensity / Mathf.Max(1, count);
        float step = count > 1 ? spreadAngle / (count - 1) : 0f;
        float start = -spreadAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            float angleRad = (start + step * i) * Mathf.Deg2Rad;
            children.Add(new ChildRaySpec(RotateVector(direction, angleRad), color, childIntensity));
        }

        return children;
    }
}

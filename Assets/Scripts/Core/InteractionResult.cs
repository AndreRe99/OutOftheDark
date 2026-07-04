using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// What a LightInteractable wants to happen to the ray that just hit it.
/// </summary>
public enum RayAction
{
    Redirect,
    Split,
    Absorb,
    PassThrough,
    Teleport
}

/// <summary>
/// Blueprint for a child ray produced by a Split (e.g. a Prism splitting white light into colors).
/// </summary>
public struct ChildRaySpec
{
    public Vector2 Direction;
    public Color Color;
    public float Intensity;

    public ChildRaySpec(Vector2 direction, Color color, float intensity)
    {
        Direction = direction;
        Color = color;
        Intensity = intensity;
    }
}

/// <summary>
/// Outcome of LightInteractable.ProcessRay, applied by the base class to the ray that triggered it.
/// </summary>
public struct InteractionResult
{
    public RayAction Action;
    public Vector2 NewDirection;
    public Color? NewColor;
    public float? NewIntensity;
    public List<ChildRaySpec> ChildRays;
    public Vector2 TargetPosition;

    public static InteractionResult Redirect(Vector2 direction, Color? newColor = null, float? newIntensity = null)
    {
        return new InteractionResult
        {
            Action = RayAction.Redirect,
            NewDirection = direction,
            NewColor = newColor,
            NewIntensity = newIntensity
        };
    }

    public static InteractionResult Absorb()
    {
        return new InteractionResult { Action = RayAction.Absorb };
    }

    public static InteractionResult PassThrough()
    {
        return new InteractionResult { Action = RayAction.PassThrough };
    }

    public static InteractionResult Split(List<ChildRaySpec> children)
    {
        return new InteractionResult { Action = RayAction.Split, ChildRays = children };
    }

    public static InteractionResult Teleport(Vector2 targetPosition, Vector2 direction)
    {
        return new InteractionResult { Action = RayAction.Teleport, TargetPosition = targetPosition, NewDirection = direction };
    }
}

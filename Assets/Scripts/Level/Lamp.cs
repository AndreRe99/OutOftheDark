using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Turns "on" when activated (typically by a Sensor): reveals fog-of-war around itself and
/// switches on its own visual/Light2D.
/// </summary>
public class Lamp : MonoBehaviour
{
    public float revealRadius = 4f;
    public float fadeDuration = 0.6f;
    public Color litColor = new Color(1f, 0.95f, 0.6f, 1f);

    private bool active;

    public void Activate()
    {
        if (active) return;
        active = true;

        FogOfWarManager.RevealCircle(transform.position, revealRadius, fadeDuration);

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = litColor;

        var light = GetComponentInChildren<Light2D>();
        if (light != null) light.enabled = true;
    }
}

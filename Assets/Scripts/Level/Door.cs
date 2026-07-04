using UnityEngine;

/// <summary>
/// Blocks light while closed (like a wall) and lets it pass through while open. Opened/closed
/// externally, typically by a Switch.
/// </summary>
public class Door : LightInteractable
{
    public bool startsOpen = false;
    public Color closedColor = new Color(0.6f, 0.6f, 0.65f, 1f);
    public Color openColor = new Color(0.6f, 0.6f, 0.65f, 0.15f);

    public bool IsOpen { get; private set; }

    void Start()
    {
        SetOpen(startsOpen);
    }

    public void SetOpen(bool open)
    {
        IsOpen = open;
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = open ? openColor : closedColor;
    }

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        return IsOpen ? InteractionResult.PassThrough() : InteractionResult.Absorb();
    }
}

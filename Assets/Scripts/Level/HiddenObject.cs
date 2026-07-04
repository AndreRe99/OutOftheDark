using UnityEngine;

/// <summary>
/// Makes the GameObject invisible and non-interactive until Reveal() is called (typically by a
/// Switch or Sensor elsewhere in the level). Attach alongside the object's normal component
/// (Mirror, LightAbsorber, LevelGoal, ...).
/// </summary>
public class HiddenObject : MonoBehaviour
{
    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        SetVisible(false);
    }

    public void Reveal()
    {
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        if (sr != null) sr.enabled = visible;
        if (col != null) col.enabled = visible;
    }
}

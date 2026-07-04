using UnityEngine;

/// <summary>
/// Wall that survives a limited number of ray hits before breaking open. Fades as it takes
/// damage; once its hit points reach zero it is destroyed, permanently opening a gap.
/// </summary>
public class DestructibleWall : LightInteractable
{
    public int hitPoints = 3;

    private int remainingHits;
    private SpriteRenderer sr;

    void Awake()
    {
        remainingHits = hitPoints;
        sr = GetComponent<SpriteRenderer>();
    }

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        remainingHits--;

        if (sr != null && hitPoints > 0)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(0.2f, 1f, (float)remainingHits / hitPoints);
            sr.color = c;
        }

        if (remainingHits <= 0)
        {
            Destroy(gameObject);
        }

        return InteractionResult.Absorb();
    }
}

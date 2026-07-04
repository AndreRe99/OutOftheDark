using UnityEngine;

/// <summary>
/// Reflects like a normal Mirror, but only survives a limited number of hits before shattering
/// and disappearing - a "use it wisely" variant of Mirror.
/// </summary>
public class FragileMirror : LightInteractable
{
    public int hitPoints = 2;
    [Range(0f, 1f)] public float glossiness = 0f;

    private int remainingHits;
    private SpriteRenderer sr;

    void Awake()
    {
        remainingHits = hitPoints;
        sr = GetComponent<SpriteRenderer>();
    }

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        Vector2 incoming = ray.GetDirection();
        Vector2 reflected = OpticsMath.Reflect(incoming, hitNormal).normalized;

        if (glossiness > 0f)
        {
            float maxAngle = Mathf.Lerp(0f, 30f, glossiness);
            float angle = Random.Range(-maxAngle, maxAngle) * Mathf.Deg2Rad;
            reflected = RotateVector(reflected, angle);
        }

        remainingHits--;
        if (sr != null && hitPoints > 0)
        {
            Color c = sr.color;
            c.a = Mathf.Lerp(0.2f, 1f, (float)remainingHits / hitPoints);
            sr.color = c;
        }

        if (remainingHits <= 0)
        {
            Destroy(gameObject, 0.05f); // let this reflection resolve before the mirror vanishes
        }

        return InteractionResult.Redirect(reflected);
    }
}

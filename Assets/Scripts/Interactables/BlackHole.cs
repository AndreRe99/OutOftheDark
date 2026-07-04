using UnityEngine;

/// <summary>
/// Creates a gravity-like field that continuously bends nearby rays toward its center; anything
/// that gets within eventHorizonRadius is destroyed. Not a LightInteractable since it needs to
/// affect rays continuously while they're nearby, not just once on first contact.
/// </summary>
public class BlackHole : MonoBehaviour
{
    public float pullRadius = 5f;
    public float pullStrength = 8f;
    public float eventHorizonRadius = 0.4f;

    void FixedUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pullRadius);
        foreach (Collider2D hit in hits)
        {
            LightRay ray = hit.GetComponent<LightRay>();
            if (ray == null) continue;

            Vector2 toCenter = (Vector2)transform.position - (Vector2)ray.transform.position;
            float distance = toCenter.magnitude;

            if (distance <= eventHorizonRadius)
            {
                ray.Terminate();
                continue;
            }

            // Gravity-like falloff: stronger pull the closer the ray is.
            float pull = pullStrength / Mathf.Max(0.5f, distance);
            Vector2 newDirection = (ray.GetDirection() + toCenter.normalized * pull * Time.fixedDeltaTime).normalized;
            ray.SetDirection(newDirection);
        }
    }
}

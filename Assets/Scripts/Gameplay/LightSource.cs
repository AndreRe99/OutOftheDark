using UnityEngine;

/// <summary>
/// Player-controlled light source: click anywhere to fire a LightRay towards the mouse position.
/// </summary>
public class LightSource : MonoBehaviour
{
    public float speed = 5f;
    public GameObject prefabToSpawn;
    public Camera cam;

    [Tooltip("Optional: if assigned, firing is capped by the level's shot budget.")]
    public ShotBudget shotBudget;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void Fire()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            Debug.LogError("No Camera available for LightSource on " + name + ".");
            return;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned to prefabToSpawn on " + name + ".");
            return;
        }

        if (shotBudget != null && !shotBudget.TryConsumeShot())
        {
            return; // out of light balls for this level
        }

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector2 spawnPos = transform.position;
        Vector2 direction = ((Vector2)mouseWorldPos - spawnPos).normalized;

        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        LightRay.CreateAndInitialize(prefabToSpawn, spawnPosition, speed, direction);
        AudioManager.Instance?.PlayShotFired();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        return cam.ScreenToWorldPoint(mousePos);
    }
}

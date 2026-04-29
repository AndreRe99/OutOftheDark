using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichtStrahlScript : MonoBehaviour
{

    public float speed = 5f;
    private Vector2 direction = Vector2.right; // Default Richtung

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float distance = speed * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name);
            // handle hit (destroy, reflect, etc.)
            Destroy(gameObject);
            return;
        }

        transform.Translate(direction * distance);
    }

    // Method to set the direction for spawned objects
    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    // Initialisierungsmethode mit Parametern
    public void Initialize(float newSpeed, Vector2 newDirection)
    {
        speed = newSpeed;
        direction = newDirection.normalized;
        Debug.Log($"LichtStrahlScript initialisiert: Speed={speed}, Direction={direction}");
    }

    // Factory-Methode: Erstellt und initialisiert das Objekt sofort
    public static GameObject CreateAndInitialize(GameObject prefab, Vector3 position, float speed, Vector2 direction)
    {
        Debug.Log("Factory-Methode aufgerufen!");
        GameObject spawnedObject = Instantiate(prefab, position, Quaternion.identity);
        Debug.Log($"Objekt erstellt: {spawnedObject.name}");
        LichtStrahlScript script = spawnedObject.GetComponent<LichtStrahlScript>();
        Debug.Log($"LichtStrahlScript gefunden: {script != null}");
        if (script != null)
        {
            script.Initialize(speed, direction);
        }
        else
        {
            Debug.LogError("LichtStrahlScript nicht gefunden auf spawned object!");
        }
        return spawnedObject;
    }
}

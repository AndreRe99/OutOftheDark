using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPunktScript : MonoBehaviour
{

    public float speed = 5f;
    private Vector2 direction;
    public GameObject prefabToSpawn; // Assign your lightray prefab here in Inspector
    public Camera cam; // optional: assign camera in inspector, falls back to Camera.main

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Detect mouse click
        if (Input.GetMouseButtonDown(0)) // 0 = Left click
        {
            if (cam == null)
            {
                cam = Camera.main;
            }

            if (cam == null)
            {
                Debug.LogError("No Camera available for StartPunktScript on " + name + ".");
                return;
            }

            // Berechne die Richtung zur Mausposition
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            Vector2 spawnPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 calculatedDirection = (new Vector2(mouseWorldPos.x, mouseWorldPos.y) - spawnPos).normalized;
            
            // Create object at this object's position
            if (prefabToSpawn != null)
            {
                // Erstelle und initialisiere sofort mit der Factory-Methode
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, 0f);
                LichtStrahlScript.CreateAndInitialize(prefabToSpawn, spawnPosition, speed, calculatedDirection);
            }
            else
            {
                Debug.LogWarning("No prefab assigned to prefabToSpawn!");
            }
        }

    }

    /// <summary>
    /// Konvertiert die aktuelle Mausposition in Weltkoordinaten
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Z-Entfernung von der Kamera für 2D
        return cam.ScreenToWorldPoint(mousePos);
    }
}

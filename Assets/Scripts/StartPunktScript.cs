using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPunktScript : MonoBehaviour
{

    public float speed = 5f;
    private Vector2 direction;
    public GameObject prefabToSpawn; // Assign your lightray prefab here in Inspector


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
            Vector3 mousePos = Input.mousePosition;
            // Calculate direction from this object to click point
            Vector3 calculatedDirection = (mousePos - transform.position).normalized;
            Vector3 center = (mousePos + transform.position) / 2f;
            Vector2 center2D = new Vector2(center.x, center.y);
            Vector2 direction2D = new Vector2(calculatedDirection.x, calculatedDirection.y);
            Debug.Log($"Direction2D: {direction2D}");

            float angle = Mathf.Atan2(calculatedDirection.y, calculatedDirection.x) * Mathf.Rad2Deg;
            Debug.Log($"Angle: {angle}");

            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            
            // Create object at this object's position, not at mouse position
            if (prefabToSpawn != null)
            {
                // Erstelle und initialisiere sofort mit der Factory-Methode
                GameObject lichtstrahl = Instantiate(prefabToSpawn, center2D, rotation);
            }
            else
            {
                Debug.LogWarning("No prefab assigned to prefabToSpawn!");
            }
        }

    }
}

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
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            
            // Calculate direction from this object to click point
            Vector2 calculatedDirection = (worldPos - transform.position).normalized;
            
            // Create object at this object's position, not at mouse position
            if (prefabToSpawn != null)
            {
                // Erstelle und initialisiere sofort mit der Factory-Methode
                LichtStrahlScript.CreateAndInitialize(prefabToSpawn, transform.position, speed, calculatedDirection);
            }
            else
            {
                Debug.LogWarning("No prefab assigned to prefabToSpawn!");
            }
        }

    }
}

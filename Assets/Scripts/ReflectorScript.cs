using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorScript : MonoBehaviour
{
    public float reflectionAngle = 45f; // Winkel des Spiegels in Grad
    public float nudgeDistance = 0.05f; // Abstand, um Re-Trigger zu vermeiden

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Wenn ein Lichtstrahl den Spiegel berührt
        LichtStrahlScript lichtStrahl = collision.GetComponent<LichtStrahlScript>();
        if (lichtStrahl != null)
        {
            // Nutze die Transform-Normale des Spiegels (transform.up) als Oberfläche-Normale
            Vector2 incoming = lichtStrahl.GetDirection();
            Vector2 normal = transform.up;
            Vector2 reflected = Vector2.Reflect(incoming, normal).normalized;

            // Setze die neue Richtung
            lichtStrahl.SetDirection(reflected);

            // Leicht wegschieben, damit der Strahl nicht sofort wieder den Trigger auslöst
            lichtStrahl.transform.position += (Vector3)(reflected * nudgeDistance);
        }
    }
}

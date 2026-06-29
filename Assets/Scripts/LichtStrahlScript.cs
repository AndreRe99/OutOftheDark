using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LichtStrahlScript : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction = Vector2.right;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Konfiguriere den Rigidbody2D für das Projektil
        rb.gravityScale = 0f; // Keine Gravität
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematisch = wird nur durch Velocity bewegt
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Füge einen Collider hinzu, wenn nicht vorhanden
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true; // Als Trigger setzen
        }

        // Mache den Lichtstrahl visuell hell
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Wenn der Lichtstrahl auf die AbsorbWand trifft, verschwindet er
        if (collision.CompareTag("AbsorbWand") || collision.name.Contains("AbsorbWand"))
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Alternative: Auch auf normale Kollisionen prüfen
        if (collision.gameObject.CompareTag("AbsorbWand") || collision.gameObject.name.Contains("AbsorbWand"))
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
        
        // Berechne den Rotationswinkel basierend auf der Richtung
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public Vector2 GetDirection()
    {
        return direction;
    }

    /// <summary>
    /// Factory-Methode zum Erstellen und Initialisieren eines Lichtstrahls
    /// </summary>
    public static LichtStrahlScript CreateAndInitialize(GameObject prefab, Vector3 position, float speed, Vector2 direction)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        LichtStrahlScript lichtStrahl = instance.GetComponent<LichtStrahlScript>();

        if (lichtStrahl == null)
        {
            lichtStrahl = instance.AddComponent<LichtStrahlScript>();
        }

        lichtStrahl.SetSpeed(speed);
        lichtStrahl.SetDirection(direction);

        return lichtStrahl;
    }
}
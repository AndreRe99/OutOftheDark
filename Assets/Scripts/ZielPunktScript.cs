using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZielPunktScript : MonoBehaviour
{
    public Color winColor = Color.green;
    public float pulseDuration = 0.8f; // wie lange die Pulse-Animation läuft
    public int pulseCount = 3; // wie oft pulsiert
    public bool pauseOnWin = false; // optional: Spiel anhalten

    private SpriteRenderer sr;
    private Color originalColor;
    private bool gameWon = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("ZielPunktScript benötigt einen SpriteRenderer auf dem GameObject: " + name);
        }
        else
        {
            originalColor = sr.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameWon) return;

        LichtStrahlScript ls = other.GetComponent<LichtStrahlScript>();
        if (ls != null)
        {
            gameWon = true;
            Destroy(other.gameObject);
            StartCoroutine(WinAndGlow());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameWon) return;

        LichtStrahlScript ls = collision.gameObject.GetComponent<LichtStrahlScript>();
        if (ls != null)
        {
            gameWon = true;
            Destroy(collision.gameObject);
            StartCoroutine(WinAndGlow());
        }
    }

    IEnumerator WinAndGlow()
    {
        if (sr == null)
            yield break;

        float singleDuration = pulseDuration / 2f;
        for (int i = 0; i < pulseCount; i++)
        {
            // Aufleuchten
            float t = 0f;
            while (t < singleDuration)
            {
                sr.color = Color.Lerp(originalColor, winColor, t / singleDuration);
                t += Time.deltaTime;
                yield return null;
            }
            sr.color = winColor;

            // Zurück
            t = 0f;
            while (t < singleDuration)
            {
                sr.color = Color.Lerp(winColor, originalColor, t / singleDuration);
                t += Time.deltaTime;
                yield return null;
            }
            sr.color = originalColor;
        }

        // Bleibe grün
        sr.color = winColor;
        Debug.Log("Gewonnen!");

        if (pauseOnWin)
        {
            Time.timeScale = 0f;
        }

        yield break;
    }
}

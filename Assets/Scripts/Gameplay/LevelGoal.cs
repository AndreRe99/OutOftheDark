using System.Collections;
using UnityEngine;

/// <summary>
/// Level end point: destroys any LightRay that reaches it and plays a win pulse animation.
/// </summary>
public class LevelGoal : MonoBehaviour
{
    public Color winColor = Color.green;
    public float pulseDuration = 0.8f;
    public int pulseCount = 3;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool gameWon = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("LevelGoal benötigt einen SpriteRenderer auf dem GameObject: " + name);
        }
        else
        {
            originalColor = sr.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        LightRay ray = other.GetComponent<LightRay>();
        if (ray != null) HandleWin(ray);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        LightRay ray = collision.gameObject.GetComponent<LightRay>();
        if (ray != null) HandleWin(ray);
    }

    private void HandleWin(LightRay ray)
    {
        if (gameWon) return;
        gameWon = true;
        ray.Terminate();

        ShotBudget shotBudget = FindObjectOfType<ShotBudget>();
        if (shotBudget != null) shotBudget.NotifyLevelWon();

        AudioManager.Instance?.PlayGoalReached();
        VfxManager.Instance?.SpawnGoalBurst(transform.position);

        StartCoroutine(WinAndGlow());
    }

    private IEnumerator WinAndGlow()
    {
        if (sr != null)
        {
            float singleDuration = pulseDuration / 2f;
            for (int i = 0; i < pulseCount; i++)
            {
                float t = 0f;
                while (t < singleDuration)
                {
                    sr.color = Color.Lerp(originalColor, winColor, t / singleDuration);
                    t += Time.deltaTime;
                    yield return null;
                }
                sr.color = winColor;

                t = 0f;
                while (t < singleDuration)
                {
                    sr.color = Color.Lerp(winColor, originalColor, t / singleDuration);
                    t += Time.deltaTime;
                    yield return null;
                }
                sr.color = originalColor;
            }

            sr.color = winColor;
        }

        LevelCompleteController completeUI = FindObjectOfType<LevelCompleteController>();
        if (completeUI != null)
        {
            completeUI.Show();
        }
    }
}

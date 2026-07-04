using System.Collections;
using UnityEngine;

/// <summary>
/// One cell of the fog-of-war grid. Starts fully opaque and fades out permanently once revealed.
/// </summary>
public class FogTile : MonoBehaviour
{
    private SpriteRenderer sr;

    public bool IsRevealed { get; private set; }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        FogOfWarManager.Register(this);
    }

    void OnDestroy()
    {
        FogOfWarManager.Unregister(this);
    }

    public void Reveal(float fadeDuration)
    {
        if (IsRevealed) return;
        IsRevealed = true;
        StartCoroutine(FadeOut(fadeDuration));
    }

    private IEnumerator FadeOut(float duration)
    {
        if (sr == null) yield break;

        Color start = sr.color;
        Color end = start;
        end.a = 0f;

        float t = 0f;
        while (t < duration)
        {
            sr.color = Color.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        sr.color = end;
        sr.enabled = false;
    }
}

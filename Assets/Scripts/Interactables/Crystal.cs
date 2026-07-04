using System.Collections;
using UnityEngine;

/// <summary>
/// What a Crystal does with a ray that hits it. Distinct from AreaRevealer (the fog-reveal
/// "light crystal" prefab flavor) - this is the optics-mechanic crystal.
/// </summary>
public enum CrystalMode
{
    Amplify,
    Redirect,
    StoreAndRelease,
    MultiRay,
    ColorChange
}

/// <summary>
/// Multi-purpose crystal whose behavior depends on `mode`: amplify intensity, redirect at a fixed
/// angle, store a ray and release it again after a delay, split into several rays, or recolor.
/// </summary>
public class Crystal : LightInteractable
{
    public CrystalMode mode = CrystalMode.Amplify;

    [Header("Amplify")]
    public float amplifyFactor = 1.5f;

    [Header("Redirect")]
    public float redirectAngle = 45f;

    [Header("Store And Release")]
    public float storeDuration = 2f;
    public GameObject rayPrefab;
    public float releaseSpeed = 5f;

    [Header("Multi Ray")]
    public int rayCount = 3;
    public float spreadAngle = 30f;

    [Header("Color Change")]
    public Color newColor = Color.cyan;

    private Coroutine releaseCoroutine;

    protected override InteractionResult ProcessRay(LightRay ray, Vector2 hitPoint, Vector2 hitNormal)
    {
        switch (mode)
        {
            case CrystalMode.Amplify:
                return InteractionResult.Redirect(ray.GetDirection(), newIntensity: Mathf.Clamp01(ray.GetIntensity() * amplifyFactor));

            case CrystalMode.Redirect:
                return InteractionResult.Redirect(RotateVector(ray.GetDirection(), redirectAngle * Mathf.Deg2Rad));

            case CrystalMode.StoreAndRelease:
                if (releaseCoroutine != null) StopCoroutine(releaseCoroutine);
                releaseCoroutine = StartCoroutine(ReleaseAfterDelay(ray.GetDirection(), ray.GetColor(), ray.GetIntensity()));
                return InteractionResult.Absorb();

            case CrystalMode.MultiRay:
                return InteractionResult.Split(BuildFan(ray.GetDirection(), ray.GetColor(), ray.GetIntensity(), rayCount, spreadAngle));

            case CrystalMode.ColorChange:
                return InteractionResult.Redirect(ray.GetDirection(), newColor);

            default:
                return InteractionResult.PassThrough();
        }
    }

    private IEnumerator ReleaseAfterDelay(Vector2 direction, Color color, float intensity)
    {
        yield return new WaitForSeconds(storeDuration);

        if (rayPrefab != null)
        {
            LightRay.CreateAndInitialize(rayPrefab, transform.position, releaseSpeed, direction, color, intensity);
        }

        releaseCoroutine = null;
    }
}

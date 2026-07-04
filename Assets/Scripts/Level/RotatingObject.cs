using UnityEngine;

/// <summary>
/// Continuously rotates its GameObject (typically a Mirror) at a constant angular speed.
/// Reflection stays correct automatically since Mirror reads transform.up fresh on every hit.
/// </summary>
public class RotatingObject : MonoBehaviour
{
    public float degreesPerSecond = 45f;
    public bool isRotating = true;

    public void SetRotating(bool rotating) => isRotating = rotating;

    void Update()
    {
        if (!isRotating) return;
        transform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);
    }
}

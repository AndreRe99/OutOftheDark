using UnityEngine;

/// <summary>
/// Lets the player manually rotate this object (typically a Mirror) with the mouse scroll wheel
/// while hovering over it - before firing a shot or anytime during play.
/// </summary>
public class PlayerRotatable : MonoBehaviour
{
    public float degreesPerScrollNotch = 15f;

    private bool hovering;

    void OnMouseEnter() => hovering = true;

    void OnMouseExit() => hovering = false;

    void Update()
    {
        if (!hovering) return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            transform.Rotate(0f, 0f, -scroll * degreesPerScrollNotch);
        }
    }
}

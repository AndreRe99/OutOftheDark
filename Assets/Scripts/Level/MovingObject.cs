using UnityEngine;

/// <summary>
/// Ping-pongs its GameObject between two world-space offsets from its starting position. Generic
/// mover: attach next to a Wall/LightAbsorber for a moving wall, or next to a Mirror for a moving
/// mirror later on.
/// </summary>
[DisallowMultipleComponent]
public class MovingObject : MonoBehaviour
{
    public Vector3 pointA = Vector3.zero;
    public Vector3 pointB = Vector3.right * 2f;
    public float speed = 1f;
    public bool isMoving = true;

    public void SetMoving(bool moving) => isMoving = moving;

    private float t;
    private Vector3 origin;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void Start()
    {
        origin = transform.position;
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        t += Time.fixedDeltaTime * speed;
        float ping = Mathf.PingPong(t, 1f);
        Vector3 target = Vector3.Lerp(origin + pointA, origin + pointB, ping);
        rb.MovePosition(target);
    }
}

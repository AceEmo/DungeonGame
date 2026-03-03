using UnityEngine;

public class GroundMovement : MonoBehaviour, IEnemyMovement
{
    [Header("Movement")]
    public float speed = 3f;
    public float steeringSmooth = 0.15f;

    [Header("Wall Avoidance")]
    public float wallCheckDistance = 1f;
    public LayerMask wallLayer;

    [Header("Separation")]
    public float separationRadius = 1f;
    public LayerMask enemyLayer;

    private Rigidbody2D rb;
    private Vector2 smoothedSteering;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 desiredDirection)
    {
        if (desiredDirection.sqrMagnitude < 0.0001f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 steering = desiredDirection.normalized;

        steering += WallAvoidance(steering);

        steering += Separation() * 0.5f;

        smoothedSteering = Vector2.Lerp(smoothedSteering, steering, steeringSmooth);

        if (smoothedSteering.sqrMagnitude > 1f)
            smoothedSteering.Normalize();

        rb.linearVelocity = smoothedSteering * speed;
    }

    private Vector2 WallAvoidance(Vector2 moveDir)
    {
        Vector2 avoid = Vector2.zero;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            moveDir,
            wallCheckDistance,
            wallLayer
        );

        if (hit.collider != null)
            avoid += hit.normal * 1f;

        Vector2 leftDir = Quaternion.Euler(0, 0, 30f) * moveDir;
        RaycastHit2D leftHit = Physics2D.Raycast(
            transform.position,
            leftDir,
            wallCheckDistance * 0.8f,
            wallLayer
        );

        if (leftHit.collider != null)
            avoid += leftHit.normal * 0.7f;

        Vector2 rightDir = Quaternion.Euler(0, 0, -30f) * moveDir;
        RaycastHit2D rightHit = Physics2D.Raycast(
            transform.position,
            rightDir,
            wallCheckDistance * 0.8f,
            wallLayer
        );

        if (rightHit.collider != null)
            avoid += rightHit.normal * 0.7f;

        return avoid;
    }

    private Vector2 Separation()
    {
        Collider2D[] others = Physics2D.OverlapCircleAll(
            transform.position,
            separationRadius,
            enemyLayer
        );

        Vector2 push = Vector2.zero;

        foreach (var col in others)
        {
            if (col.transform == transform) continue;

            Vector2 diff = (Vector2)(transform.position - col.transform.position);
            float dist = diff.magnitude;

            if (dist > 0.001f)
                push += diff.normalized / dist;
        }

        return push;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }
}
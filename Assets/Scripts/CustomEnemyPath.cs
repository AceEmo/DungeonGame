using UnityEngine;
using System.Collections.Generic;

public class CustomEnemyPath : MonoBehaviour
{
    
    public float Speed = 3f;
    public float RebuildTime = 0.3f;
    public float PointDistance = 1f;
    public float ObstacleAvoidanceForce = 1.5f;
    public Animator animator;
    private Vector2 currentDirection;
    private float directionLockTimer = 0f;
    public float DirectionLockTime = 0.2f;
    private Transform player;
    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;

    private float rebuildTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        rebuildTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (player == null || enemyHealth == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        directionLockTimer -= Time.fixedDeltaTime;

        if (directionLockTimer <= 0f)
        {
            currentDirection = GetSmartDirection();
            directionLockTimer = DirectionLockTime;
        }

        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 0.6f);
        Vector2 separation = Vector2.zero;

        foreach (var col in nearby)
        {
            if (col != null &&
                col.gameObject != gameObject &&
                col.CompareTag("Enemy"))
            {
                separation += (Vector2)(transform.position - col.transform.position);
            }
        }

        if (separation != Vector2.zero)
            separation = separation.normalized * ObstacleAvoidanceForce;

        Vector2 finalVelocity = (currentDirection + separation).normalized * Speed;

        rb.linearVelocity = finalVelocity;

        if (animator != null)
        {
            animator.SetFloat("MoveX", finalVelocity.x);
            animator.SetFloat("MoveY", finalVelocity.y);
        }
    }

    private Vector2 GetSmartDirection()
    {
        Vector2 directionToPlayer = ((Vector2)player.position - rb.position).normalized;

        RaycastHit2D directHit = Physics2D.Raycast(
            rb.position,
            directionToPlayer,
            PointDistance,
            LayerMask.GetMask("Wall")
        );

        if (directHit.collider == null)
            return directionToPlayer;

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector2 checkDir = Quaternion.Euler(0, 0, angle) * directionToPlayer;

            RaycastHit2D hit = Physics2D.Raycast(
                rb.position,
                checkDir,
                PointDistance,
                LayerMask.GetMask("Wall")
            );

            if (hit.collider == null)
                return checkDir.normalized;
        }

        return Vector2.zero;
    }
}
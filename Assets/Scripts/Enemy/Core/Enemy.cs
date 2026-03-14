using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData data;

    private IEnemyMovement movement;
    private IEnemyBehaviour behaviour;

    private Animator animator;
    private Rigidbody2D rb;
    private EnemyHealth health;

    public EnemyData Data => data;

    private void Awake()
    {
        movement = GetComponent<IEnemyMovement>();
        behaviour = GetComponent<IEnemyBehaviour>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
    }

    private void FixedUpdate()
    {
        Vector2 direction = behaviour.GetDirection();
        movement.Move(direction);
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (animator == null || rb == null || health == null) return;

        if (health.IsEnemyDead()) return;

        Vector2 velocity = rb.linearVelocity;

        animator.SetFloat("MoveX", velocity.x);
        animator.SetFloat("MoveY", velocity.y);
    }
}
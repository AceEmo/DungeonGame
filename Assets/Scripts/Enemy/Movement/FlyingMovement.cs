using UnityEngine;
public class FlyingMovement : MonoBehaviour, IEnemyMovement
{
    private Enemy enemy;
    private Rigidbody2D rb;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction)
    {
        rb.linearVelocity = direction * enemy.Data.speed;
    }
}
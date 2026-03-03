using UnityEngine;
public class FlyingMovement : MonoBehaviour, IEnemyMovement
{
    public float speed = 3f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction)
    {
        rb.linearVelocity = direction * speed;
    }
}
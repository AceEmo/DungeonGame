using UnityEngine;

public class BossMovement
{
    private Rigidbody2D rigidbody;

    public BossMovement(Rigidbody2D rb)
    {
        rigidbody = rb;
    }

    public void Move(Vector2 direction, float speed)
    {
        rigidbody.linearVelocity = direction * speed;
    }

    public void Stop()
    {
        rigidbody.linearVelocity = Vector2.zero;
    }
}
using UnityEngine;

public class RandomFlyingBehaviour : MonoBehaviour, IEnemyBehaviour
{
    public float changeDirectionInterval = 2f;

    private float timer;
    private Vector2 currentDirection;

    private void Start()
    {
        PickNewDirection();
    }

    public Vector2 GetDirection()
    {
        timer += Time.deltaTime;

        if (timer >= changeDirectionInterval)
        {
            PickNewDirection();
            timer = 0f;
        }

        return currentDirection;
    }

    private void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        currentDirection = new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle)
        ).normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;

            currentDirection = Vector2.Reflect(currentDirection, normal).normalized;

            timer = 0f;
        }
    }
}

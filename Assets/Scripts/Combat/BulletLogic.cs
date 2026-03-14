using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

    private int damage;
    private bool hasHit = false;

    public void SetDamage(int value)
    {
        damage = value;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                hasHit = true;
                damageable.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
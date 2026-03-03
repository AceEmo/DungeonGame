using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damageAmount = 1f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damageAmount, transform.position);
        }
    }
}
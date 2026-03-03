using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public event System.Action<EnemyHealth> OnEnemyDied;

    public int MaxHealth = 3;
    public Color HitColor = new Color(1f, 0.5f, 0.5f);
    public Animator Animator;

    private int CurrentHealth;
    private bool IsDead = false;
    private SpriteRenderer Sr;
    private Collider2D[] Colliders;

    private Color OriginalColor;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        Sr = GetComponent<SpriteRenderer>();
        Colliders = GetComponents<Collider2D>();
        if (Sr != null) OriginalColor = Sr.color;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        CurrentHealth -= amount;
        StopCoroutine("HitFlash");
        StartCoroutine(HitFlash());

        if (Animator != null)
            Animator.SetTrigger("Hit");

        if (CurrentHealth <= 0)
            Die();
    }

    private IEnumerator HitFlash()
    {
        if (Sr == null) yield break;
        Sr.color = HitColor;
        yield return new WaitForSeconds(0.1f);
        Sr.color = OriginalColor;
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        StopCoroutine("HitFlash");

        if (Animator != null)
            Animator.SetTrigger("Die");

        if (Colliders != null)
        {
            foreach (Collider2D col in Colliders)
                col.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        IEnemyMovement movement = GetComponent<IEnemyMovement>();
        if (movement != null)
            ((MonoBehaviour)movement).enabled = false;

        EnemyDamage damage = GetComponent<EnemyDamage>();
        if (damage != null)
            damage.enabled = false;

        EnemyAnimation animation = GetComponent<EnemyAnimation>();
        if (animation != null)
            animation.enabled = false;

        StartCoroutine(FadeAndDestroy());
        OnEnemyDied?.Invoke(this);
    }

    private IEnumerator FadeAndDestroy()
    {
        float duration = 1.5f;
        float t = 0f;
        Color startColor = Sr != null ? Sr.color : Color.white;

        while (t < duration)
        {
            t += Time.deltaTime;
            if (Sr != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, t / duration);
                Sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }
            yield return null;
        }

        Destroy(gameObject);
    }
    public bool IsEnemyDead() => IsDead;
}
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    public event System.Action<EnemyHealth> OnEnemyDied;

    private Enemy enemy;

    public Animator Animator;

    private int currentHealth;
    private Color hitColor;
    private bool isDead;
    private SpriteRenderer spriteRenderer;
    private Collider2D[] colliders;
    private Coroutine hitFlashCoroutine;

    private Color originalColor;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliders = GetComponents<Collider2D>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    private void Start()
    {
        if (!ValidateRequiredComponents())
        {
            enabled = false;
            return;
        }

        currentHealth = enemy.CurrentMaxHealth;
        hitColor = enemy.Data.hitColor;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(HitFlash());

        if (Animator != null)
            Animator.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer == null) yield break;
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
        hitFlashCoroutine = null;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
            hitFlashCoroutine = null;
        }

        if (Animator != null)
        {
            Animator.ResetTrigger("Hit");
            Animator.SetTrigger("Die");
        }

        if (colliders != null)
        {
            foreach (Collider2D col in colliders)
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

        Enemy enemyComponent = GetComponent<Enemy>();
        if (enemyComponent != null)
            enemyComponent.enabled = false;

        StartCoroutine(FadeAndDestroy());
        OnEnemyDied?.Invoke(this);
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return SpriteFadeHelper.FadeSpriteRenderer(
            spriteRenderer,
            SpriteFadeHelper.DefaultFadeDuration,
            () => Destroy(gameObject));
    }

    public bool IsEnemyDead() => isDead;

    private bool ValidateRequiredComponents()
    {
        if (enemy != null && enemy.Data != null)
        {
            return true;
        }

        Debug.LogError($"{nameof(EnemyHealth)} on {name} requires {nameof(Enemy)} with assigned {nameof(EnemyData)}.", this);
        return false;
    }
}
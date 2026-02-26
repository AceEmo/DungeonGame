using UnityEngine;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float knockbackForce = 8f;

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;

    private float currentHealth;
    private bool isInvincible;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => stats.maxHealth;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Collider2D col;
    private PlayerMovement movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        movement = GetComponent<PlayerMovement>();

        currentHealth = stats.startHealth;

        OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(1, Vector3.zero);
    }

    public void TakeDamage(float amount, Vector2 source)
    {
        if (isInvincible || currentHealth <= 0)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);

        ApplyKnockback(source);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Invincibility());
    }

    private void ApplyKnockback(Vector2 source)
    {
        Vector2 direction = (transform.position - (Vector3)source).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        SetAlpha(0.5f);
        yield return new WaitForSeconds(invincibilityDuration);
        SetAlpha(1f);
        isInvincible = false;
    }

    private void SetAlpha(float alpha)
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }

    public void ResetHealth()
    {
        currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(currentHealth, stats.maxHealth);
    }

    private void Die()
    {
        if (movement != null) movement.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (col != null) col.enabled = false;

        if (animator != null)
            animator.SetTrigger("Die");

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        OnPlayerDied?.Invoke();
    }
}
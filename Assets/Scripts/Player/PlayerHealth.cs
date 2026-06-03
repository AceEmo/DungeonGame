using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    [Serializable]
    public struct AudioConfig
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Header("Dependencies")]
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerMovement movement;

    [Header("Settings")]
    [SerializeField] private float invincibilityDuration = 1f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float deathSequenceDelay = 1.5f;

    [Header("Audio Configurations")]
    [SerializeField] private AudioConfig hitAudio = new AudioConfig { volume = 1f };
    [SerializeField] private AudioConfig healAudio = new AudioConfig { volume = 1f };
    [SerializeField] private AudioConfig dieAudio = new AudioConfig { volume = 1f };

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDied;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => stats != null ? stats.maxHealth : 0f;
    public bool IsDead => CurrentHealth <= 0;

    private bool isInvincible;
    private const float DamagedVisualAlpha = 0.5f;
    private const float NormalVisualAlpha = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Collider2D col;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        
        if (movement == null) movement = GetComponent<PlayerMovement>();

        if (!ValidateStats())
        {
            enabled = false;
        }
    }

    private void Start()
    {
        InitializeHealth();
    }

    public void ResetHealth()
    {
        InitializeHealth();
        ResetAnimations();
    }

    public void TakeDamage(float amount, Vector2 damageSourcePosition)
    {
        if (stats == null) return;
        if (isInvincible || IsDead) return;

        ReduceHealth(amount);
        PlayHitFeedback(damageSourcePosition);

        if (IsDead)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilitySequence());
    }

    public void Heal(float amount)
    {
        if (stats == null) return;
        if (IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, stats.maxHealth);
        PlaySound(healAudio);
        NotifyHealthChanged();
    }

    public void ApplyStats()
    {
        if (stats == null) return;

        if (CurrentHealth > stats.maxHealth)
        {
            CurrentHealth = stats.maxHealth;
        }
        NotifyHealthChanged();
    }

    private void InitializeHealth()
    {
        if (stats == null) return;

        CurrentHealth = stats.startHealth;
        isInvincible = false;

        ConfigureComponentsForLife();
        NotifyHealthChanged();
    }

    private void ConfigureComponentsForLife()
    {
        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
        if (col != null) col.enabled = true;
        if (movement != null) movement.enabled = true;
    }

    private void ReduceHealth(float amount)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        NotifyHealthChanged();
    }

    private void PlayHitFeedback(Vector2 damageSourcePosition)
    {
        ApplyKnockback(damageSourcePosition);
        PlaySound(hitAudio);
    }

    private void PlaySound(AudioConfig config)
    {
        if (audioSource != null && config.clip != null)
        {
            audioSource.PlayOneShot(config.clip, config.volume);
        }
    }

    private void ApplyKnockback(Vector2 source)
    {
        if (rb == null) return;
        
        Vector2 direction = (transform.position - (Vector3)source).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator InvincibilitySequence()
    {
        isInvincible = true;
        SetSpriteAlpha(DamagedVisualAlpha);
        
        yield return new WaitForSeconds(invincibilityDuration);
        
        SetSpriteAlpha(NormalVisualAlpha);
        isInvincible = false;
    }

    private void SetSpriteAlpha(float alpha)
    {
        if (sr == null) return;
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }

    private void Die()
    {
        DisableComponentsOnDeath();
        PlaySound(dieAudio);
        if (animator != null) animator.SetTrigger("Die");

        StartCoroutine(DeathDelaySequence());
    }

    private void DisableComponentsOnDeath()
    {
        if (movement != null) movement.enabled = false;
        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private IEnumerator DeathDelaySequence()
    {
        yield return new WaitForSecondsRealtime(deathSequenceDelay);
        OnPlayerDied?.Invoke();
    }

    private void ResetAnimations()
    {
        if (animator == null) return;

        animator.ResetTrigger("Die");
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", 0f);
        animator.SetFloat("Speed", 0f);
        animator.Play("IdleTree", 0, 0f);
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    private bool ValidateStats()
    {
        if (stats != null)
        {
            return true;
        }

        Debug.LogError($"{nameof(PlayerHealth)} on {name} requires {nameof(PlayerStats)}.", this);
        return false;
    }
}
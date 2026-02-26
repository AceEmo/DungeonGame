using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private Color hitColor = new Color(1f, 0.5f, 0.5f);
    [SerializeField] private Animator animator;

    private int currentHealth;
    private bool isDead;
    private SpriteRenderer sr;
    private Collider2D col;
    private MonoBehaviour movementScript;
    private Color originalColor;

    private void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        movementScript = GetComponent<MonoBehaviour>();
        if (sr != null) originalColor = sr.color;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        StopCoroutine("HitFlash");
        StartCoroutine(HitFlash());

        if (animator != null)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator HitFlash()
    {
        if (sr == null) yield break;

        sr.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StopCoroutine("HitFlash");

        if (animator != null)
            animator.SetTrigger("Die");

        if (movementScript != null)
            movementScript.enabled = false;

        if (col != null)
            col.enabled = false;

        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        float duration = 1.5f;
        float t = 0f;
        Color startColor = sr != null ? sr.color : Color.white;

        while (t < duration)
        {
            t += Time.deltaTime;

            if (sr != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, t / duration);
                sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
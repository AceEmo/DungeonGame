using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour, IDamageable
{
    public BossData data;

    [Header("Attack Points")]
    public Transform attackPointUp;
    public Transform attackPointDown;
    public Transform attackPointLeft;
    public Transform attackPointRight;

    [Header("FSM")]
    private IBossState currentState;

    private int currentHealth;
    private bool isDead = false;
    private Vector2 lastMoveDir = Vector2.down;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Color originalColor;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr != null ? sr.color : Color.white;
        currentHealth = data.MaxHealth;
    }

    private void Start()
    {
        SetState(new IdleState());
    }

    private void Update()
    {
        if (!isDead)
            currentState?.UpdateState(this);
    }

    public void SetState(IBossState newState)
    {
        if (currentState != null)
            currentState.ExitState(this);

        currentState = newState;
        currentState.EnterState(this);
    }

    public void Move(Vector2 direction)
    {
        rb.linearVelocity = direction * data.speed;
        if (direction != Vector2.zero)
            lastMoveDir = direction;
        UpdateAnimator(direction);
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        UpdateAnimator(Vector2.zero);
    }

    public void UpdateAnimator(Vector2 moveDir)
    {
        animator.SetFloat("MoveX", moveDir.x != 0 ? moveDir.x : lastMoveDir.x);
        animator.SetFloat("MoveY", moveDir.y != 0 ? moveDir.y : lastMoveDir.y);
    }

    public Transform Player => player;

    public bool IsDead => isDead;
    public Animator AnimatorComponent => animator;

    public BossData Data => data;

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
        sr.color = data.hitColor;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StopCoroutine("HitFlash");

        if (animator != null)
        {
            animator.ResetTrigger("Hit");
            animator.SetTrigger("Die");
        }

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
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

    private Transform GetCurrentAttackPoint()
    {
        if (Mathf.Abs(lastMoveDir.x) > Mathf.Abs(lastMoveDir.y))
        {
            if (lastMoveDir.x > 0)
                return attackPointRight;
            else
                return attackPointLeft;
        }
        else
        {
            if (lastMoveDir.y > 0)
                return attackPointUp;
            else
                return attackPointDown;
        }
    }

    public void DealDamageToPlayer()
    {
        Transform point = GetCurrentAttackPoint();
        if (point == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, data.attackRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(data.attackDamage, transform.position);
            }
        }
    }
}
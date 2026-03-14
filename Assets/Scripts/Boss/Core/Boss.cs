using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour, IDamageable
{
    public BossData data;
    public event System.Action OnBossDied;

    [Header("Attack Points")]
    public Transform attackPointUp;
    public Transform attackPointDown;
    public Transform attackPointLeft;
    public Transform attackPointRight;

    [Header("FSM")]
    private IBossState currentState;

    private int currentHealth;
    private float currentSpeed;
    private int currentDamage;
    private bool isDead = false;
    private bool isRaging = false;
    private float lastDashTime = -999f;

    private Vector2 lastMoveDir = Vector2.down;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    private Color originalColor;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        materialPropertyBlock = new MaterialPropertyBlock();
        currentHealth = data.MaxHealth;
        currentSpeed = data.speed;
        currentDamage = data.attackDamage;
    }

    private void Start()
    {
        SetState(new IdleState());
    }

    private void Update()
    {
        if (!isDead)
            currentState?.UpdateState(this);
        CheckRage();
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
        rigidBody.linearVelocity = direction * currentSpeed;
        if (direction != Vector2.zero)
            lastMoveDir = direction;
        UpdateAnimator(direction);
    }

    public void StopMoving()
    {
        rigidBody.linearVelocity = Vector2.zero;
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
    public float LastDashTime => lastDashTime;
    
    public void StartDashCooldown()
    {
        lastDashTime = Time.time;
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
        if (spriteRenderer == null) yield break;

        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        Color baseColor = isRaging ? data.rageColor : originalColor;

        Color flashColor = Color.Lerp(baseColor, data.hitColor, 0.7f);
        materialPropertyBlock.SetColor("_Color", flashColor);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);

        yield return new WaitForSeconds(0.1f);

        materialPropertyBlock.SetColor("_Color", baseColor);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
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

        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.simulated = false;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
            col.enabled = false;

        StartCoroutine(FadeAndDestroy());
        OnBossDied?.Invoke();
    }

    private IEnumerator FadeAndDestroy()
    {
        float duration = 1.5f;
        float t = 0f;
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        while (t < duration)
        {
            t += Time.deltaTime;
            if (spriteRenderer != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, t / duration);
                spriteRenderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetColor("_Color", new Color(startColor.r, startColor.g, startColor.b, alpha));
                spriteRenderer.SetPropertyBlock(materialPropertyBlock);
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
                PlayerHealth playerHitBox = hit.GetComponent<PlayerHealth>();
                if (playerHitBox != null)
                    playerHitBox.TakeDamage(currentDamage, transform.position);
            }
        }
    }
    private void CheckRage()
    {
        if (isRaging) return;

        float hpPercent = (float)currentHealth / data.MaxHealth;
        if (hpPercent <= data.rageThreshold)
        {
            isRaging = true;

            currentSpeed = data.speed * data.rageSpeedMultiplier;
            currentDamage = Mathf.RoundToInt(data.attackDamage * data.rageDamageMultiplier);

            if (spriteRenderer != null)
            {
                spriteRenderer.GetPropertyBlock(materialPropertyBlock);
                materialPropertyBlock.SetColor("_Color", data.rageColor);
                spriteRenderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }
}
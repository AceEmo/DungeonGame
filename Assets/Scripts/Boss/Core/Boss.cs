using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Boss : MonoBehaviour, IDamageable
{
    [Header("Boss Settings")]
    public BossData data;

    [Header("Attack Points")]
    public Transform attackPointUp;
    public Transform attackPointDown;
    public Transform attackPointLeft;
    public Transform attackPointRight;

    public event Action OnBossDied;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock materialPropertyBlock;
    private Color originalColor;

    private BossBrain brain;
    private BossContext context;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform playerTarget;

    private int currentMaxHealth;
    private float currentSpeed;
    private float currentDamage;

    private void Awake()
    {
        InitializeComponents();
        if (!ValidatePrefabSetup())
        {
            enabled = false;
            return;
        }

        ApplyDifficultyMultiplier();
        InitializeContext();
        InitializeBrain();
    }

    private void Start()
    {
        brain?.Start();
    }

    private void Update()
    {
        brain?.Update();
    }

    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        materialPropertyBlock = new MaterialPropertyBlock();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void ApplyDifficultyMultiplier()
    {
        if (data == null)
        {
            currentMaxHealth = 20;
            currentSpeed = 3f;
            currentDamage = 2;
            return;
        }

        DifficultyScaler.ScaledStats stats = DifficultyScaler.Scale(
            data.MaxHealth, data.speed, data.attackDamage);

        currentMaxHealth = stats.MaxHealth;
        currentSpeed = stats.Speed;
        currentDamage = stats.Damage;
    }

    private void InitializeContext()
    {
        context = new BossContext
        {
            BossTransform = transform,
            Player = playerTarget,
            Data = data,

            Animator = animator,
            SpriteRenderer = spriteRenderer,

            CurrentSpeed = currentSpeed,
            CurrentDamage = Mathf.RoundToInt(currentDamage),

            Health = new BossHealth(currentMaxHealth),
            Movement = new BossMovement(rb),
            Rage = new BossRage(),

            Combat = new BossCombat(
                attackPointUp,
                attackPointDown,
                attackPointLeft,
                attackPointRight)
        };
    }

    private void InitializeBrain()
    {
        brain = new BossBrain(context);
        context.Brain = brain;
    }

    public void TakeDamage(int amount)
    {
        if (context == null)
            return;

        if (context.IsDead)
            return;

        context.Health.TakeDamage(amount);
        StartCoroutine(HitFlash());

        if (context.Health.IsDead)
        {
            HandleDeath();
        }
        else
        {
            HandleHit();
        }
    }

    private void HandleDeath()
    {
        context.Brain.ChangeState(new DeathState());
        OnBossDied?.Invoke();
    }

    private void HandleHit()
    {
        context.Animator?.SetTrigger("Hit");
    }

    private IEnumerator HitFlash()
    {
        Color flashColor = Color.Lerp(originalColor, data.hitColor, 0.7f);

        SetSpriteColor(flashColor);
        yield return new WaitForSeconds(0.1f);
        SetSpriteColor(originalColor);
    }

    private void SetSpriteColor(Color color)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor("_Color", color);
        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }

    public void StartFadeAndDestroy()
    {
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        yield return SpriteFadeHelper.FadeMaterialPropertyBlock(
            spriteRenderer,
            materialPropertyBlock,
            SpriteFadeHelper.DefaultFadeDuration,
            () => Destroy(gameObject));
    }

    private bool ValidatePrefabSetup()
    {
        bool isValid = true;

        if (spriteRenderer == null)
        {
            Debug.LogError($"{nameof(Boss)} on {name} requires {nameof(SpriteRenderer)}.", this);
            isValid = false;
        }

        if (rb == null)
        {
            Debug.LogError($"{nameof(Boss)} on {name} requires {nameof(Rigidbody2D)}.", this);
            isValid = false;
        }

        if (animator == null)
        {
            Debug.LogError($"{nameof(Boss)} on {name} requires {nameof(Animator)}.", this);
            isValid = false;
        }

        if (data == null)
        {
            Debug.LogError($"{nameof(Boss)} on {name} requires {nameof(BossData)}.", this);
            isValid = false;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTarget = playerObject != null ? playerObject.transform : null;
        if (playerTarget == null)
        {
            Debug.LogError($"{nameof(Boss)} on {name} requires a scene object tagged Player.", this);
            isValid = false;
        }

        if (attackPointUp == null || attackPointDown == null || attackPointLeft == null || attackPointRight == null)
        {
            Debug.LogError($"{nameof(Boss)} on {name} requires all attack points to be assigned.", this);
            isValid = false;
        }

        return isValid;
    }
}
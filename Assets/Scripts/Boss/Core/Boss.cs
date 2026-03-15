using UnityEngine;
using System;
using System.Collections;

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

    private void Awake()
    {
        InitializeComponents();
        InitializeContext();
        InitializeBrain();
    }

    private void Start()
    {
        brain.Start();
    }

    private void Update()
    {
        brain.Update();
    }

    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        materialPropertyBlock = new MaterialPropertyBlock();
        originalColor = spriteRenderer.color;
    }

    private void InitializeContext()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;

        context = new BossContext
        {
            BossTransform = transform,
            Player = player,
            Data = data,

            Animator = GetComponent<Animator>(),
            SpriteRenderer = spriteRenderer,

            Health = new BossHealth(data.MaxHealth),
            Movement = new BossMovement(GetComponent<Rigidbody2D>()),
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
        context.Animator.SetTrigger("Hit");
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
        float duration = 1.5f;
        float animationTime = 0f;

        Color startColor = spriteRenderer.color;

        while (animationTime < duration)
        {
            animationTime += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, animationTime / duration);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);

            SetSpriteColor(newColor);
            yield return null;
        }

        Destroy(gameObject);
    }
}
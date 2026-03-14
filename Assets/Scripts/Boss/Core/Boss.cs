using UnityEngine;
using System;
using System.Collections;

public class Boss : MonoBehaviour, IDamageable
{
    public BossData data;
    private SpriteRenderer spriteRenderer;

    public Transform attackPointUp;
    public Transform attackPointDown;
    public Transform attackPointLeft;
    public Transform attackPointRight;

    public event Action OnBossDied;

    private BossBrain brain;
    private BossContext context;

    private MaterialPropertyBlock materialPropertyBlock;
    private Color originalColor;

    private void Awake()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Animator animator = GetComponent<Animator>();
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        materialPropertyBlock = new MaterialPropertyBlock();
        originalColor = sprite.color;

        context = new BossContext
        {
            BossTransform = transform,
            Player = player,
            Data = data,

            Animator = animator,
            SpriteRenderer = sprite,

            Health = new BossHealth(data.MaxHealth),
            Movement = new BossMovement(rb),
            Rage = new BossRage(),

            Combat = new BossCombat(
                attackPointUp,
                attackPointDown,
                attackPointLeft,
                attackPointRight)
        };

        brain = new BossBrain(context);
        context.Brain = brain;
    }

    private void Start()
    {
        brain.Start();
    }

    private void Update()
    {
        brain.Update();
    }

    public void TakeDamage(int amount)
    {
        if (context.IsDead)
            return;

        context.Health.TakeDamage(amount);

        StartCoroutine(HitFlash());

       if (context.Health.IsDead)
        {
            context.Brain.ChangeState(new DeathState());
            OnBossDied?.Invoke();
        } else
        {
            context.Animator.SetTrigger("Hit");
        }
    }

    private System.Collections.IEnumerator HitFlash()
    {
        context.SpriteRenderer.GetPropertyBlock(materialPropertyBlock);

        Color flashColor =
            Color.Lerp(originalColor, data.hitColor, 0.7f);

        materialPropertyBlock.SetColor("_Color", flashColor);
        context.SpriteRenderer.SetPropertyBlock(materialPropertyBlock);

        yield return new WaitForSeconds(0.1f);

        materialPropertyBlock.SetColor("_Color", originalColor);
        context.SpriteRenderer.SetPropertyBlock(materialPropertyBlock);
    }
    public void StartFadeAndDestroy()
    {
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        float duration = 1.5f;
        float t = 0f;
        Color startColor = spriteRenderer.color;

        while (t < duration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, t / duration);
            spriteRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_Color", new Color(startColor.r, startColor.g, startColor.b, alpha));
            spriteRenderer.SetPropertyBlock(materialPropertyBlock);

            yield return null;
        }

        Destroy(gameObject);
    }
}
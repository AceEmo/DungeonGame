using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;

    private IEnemyMovement movement;
    private IEnemyBehaviour behaviour;

    private Animator animator;
    private Rigidbody2D rb;
    private EnemyHealth health;

    private int currentMaxHealth;
    private float currentSpeed;
    private float currentDamage;

    public EnemyData Data => data;
    public int CurrentMaxHealth => currentMaxHealth;
    public float CurrentSpeed => currentSpeed;
    public float CurrentDamage => currentDamage;

    private void Awake()
    {
        InitializeComponents();
        if (!ValidateRequiredComponents())
        {
            enabled = false;
            return;
        }

        ApplyDifficultyMultiplier();
    }

    private void FixedUpdate()
    {
        movement.Move(behaviour.GetDirection());
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void InitializeComponents()
    {
        movement = GetComponent<IEnemyMovement>();
        behaviour = GetComponent<IEnemyBehaviour>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
    }

    private void ApplyDifficultyMultiplier()
    {
        DifficultyScaler.ScaledStats stats = DifficultyScaler.Scale(
            data.MaxHealth, data.speed, Mathf.RoundToInt(data.damage));

        currentMaxHealth = stats.MaxHealth;
        currentSpeed = stats.Speed;
        currentDamage = stats.Damage;
    }

    private bool ValidateRequiredComponents()
    {
        bool isValid = true;

        if (data == null)
        {
            Debug.LogError($"{nameof(Enemy)} on {name} requires {nameof(EnemyData)}.", this);
            isValid = false;
        }

        if (movement == null)
        {
            Debug.LogError($"{nameof(Enemy)} on {name} requires an {nameof(IEnemyMovement)} component.", this);
            isValid = false;
        }

        if (behaviour == null)
        {
            Debug.LogError($"{nameof(Enemy)} on {name} requires an {nameof(IEnemyBehaviour)} component.", this);
            isValid = false;
        }

        return isValid;
    }

    private void UpdateAnimation()
    {
        if (animator == null || rb == null || health == null) return;
        if (health.IsEnemyDead()) return;

        Vector2 velocity = rb.linearVelocity;

        animator.SetFloat("MoveX", velocity.x);
        animator.SetFloat("MoveY", velocity.y);
    }
}
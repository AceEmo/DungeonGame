using UnityEngine;

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
        ApplyDifficultyMultiplier();
    }

    private void FixedUpdate()
    {
        Vector2 direction = behaviour.GetDirection();
        movement.Move(direction);
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
        if (data == null || GameManager.Instance == null)
        {
            currentMaxHealth = data != null ? data.MaxHealth : 3;
            currentSpeed = data != null ? data.speed : 3f;
            currentDamage = data != null ? Mathf.RoundToInt(data.damage) : 1;
            return;
        }

        float multiplier = GameManager.Instance.Settings.Difficulty.GetStatMultiplier();

        currentMaxHealth = Mathf.RoundToInt(data.MaxHealth * multiplier);
        currentDamage = Mathf.RoundToInt(data.damage * multiplier);
        currentSpeed = data.speed * multiplier; 
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
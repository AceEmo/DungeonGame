public class BossHealth
{
    private int currentHealth;

    public int MaxHealth { get; }

    public bool IsDead => currentHealth <= 0;

    public BossHealth(int maxHealth)
    {
        MaxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        currentHealth -= amount;

        if (currentHealth < 0)
            currentHealth = 0;
    }

    public float HealthPercent()
    {
        return (float)currentHealth / MaxHealth;
    }
}

using UnityEngine;

public static class DifficultyScaler
{
    public struct ScaledStats
    {
        public int MaxHealth;
        public float Speed;
        public int Damage;
    }

    public static ScaledStats Scale(int baseHealth, float baseSpeed, int baseDamage)
    {
        if (GameManager.Instance == null)
        {
            return new ScaledStats
            {
                MaxHealth = baseHealth,
                Speed = baseSpeed,
                Damage = baseDamage
            };
        }

        float multiplier = GameManager.Instance.Settings.Difficulty.GetStatMultiplier();

        return new ScaledStats
        {
            MaxHealth = Mathf.RoundToInt(baseHealth * multiplier),
            Speed = baseSpeed * multiplier,
            Damage = Mathf.RoundToInt(baseDamage * multiplier)
        };
    }
}

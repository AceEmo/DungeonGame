using UnityEngine;

public class BossRage
{
    private bool raging;

    public bool IsRaging => raging;

    public void UpdateRage(BossContext context)
    {
        if (raging)
            return;

        float hpPercent = context.Health.HealthPercent();

        if (hpPercent <= context.Data.rageThreshold)
        {
            raging = true;

            // НЕ променяме context.Data, само инстанцията
            context.CurrentSpeed = context.Data.speed * context.Data.rageSpeedMultiplier;
            context.CurrentDamage = Mathf.RoundToInt(
                context.Data.attackDamage * context.Data.rageDamageMultiplier);

            if (context.SpriteRenderer != null)
                context.SpriteRenderer.color = context.Data.rageColor;
        }
    }
}
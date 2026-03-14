using UnityEngine;

public class BossCombat
{
    private Transform attackUp;
    private Transform attackDown;
    private Transform attackLeft;
    private Transform attackRight;

    public BossCombat(
        Transform up,
        Transform down,
        Transform left,
        Transform right)
    {
        attackUp = up;
        attackDown = down;
        attackLeft = left;
        attackRight = right;
    }

    public void DealDamage(BossContext context)
    {
        Transform point = GetAttackPoint(context.LastMoveDirection);

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                point.position,
                context.Data.attackRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.GetComponent<PlayerHealth>();

                if (player != null)
                    player.TakeDamage(
                        context.Data.attackDamage,
                        context.BossTransform.position);
            }
        }
    }

    private Transform GetAttackPoint(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? attackRight : attackLeft;
        }

        return dir.y > 0 ? attackUp : attackDown;
    }
}

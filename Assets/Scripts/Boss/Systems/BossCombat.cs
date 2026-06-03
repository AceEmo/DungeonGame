using UnityEngine;

public class BossCombat
{
    private const int MaxAttackHits = 8;

    private readonly Transform attackUp;
    private readonly Transform attackDown;
    private readonly Transform attackLeft;
    private readonly Transform attackRight;
    private readonly Collider2D[] attackHits = new Collider2D[MaxAttackHits];

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

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            point.position,
            context.Data.attackRadius);

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null) continue;

            if (hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.GetComponent<PlayerHealth>();

                if (player != null)
                    player.TakeDamage(
                        context.CurrentDamage,
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
using UnityEngine;

public class BossContext
{
    public Transform BossTransform;
    public Transform Player;

    public BossData Data;

    public BossHealth Health;
    public BossMovement Movement;
    public BossCombat Combat;
    public BossRage Rage;
    public float CurrentSpeed;
    public int CurrentDamage;

    public BossBrain Brain;

    public Animator Animator;
    public SpriteRenderer SpriteRenderer;

    public float LastDashTime;

    public Vector2 LastMoveDirection = Vector2.down;
    public bool IsDead => Health.IsDead;
}
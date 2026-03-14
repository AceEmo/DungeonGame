using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Boss/Boss Data")]
public class BossData : ScriptableObject
{
    public int MaxHealth = 20;

    public float speed = 3f;
    public float attackRange = 2f;
    public int attackDamage = 2;

    public float attackRadius = 0.8f;

    public Color hitColor = new Color(1f, 0.5f, 0.5f);

    [Header("Rage")]
    public float rageThreshold = 0.3f;
    public float rageSpeedMultiplier = 1.5f;
    public float rageDamageMultiplier = 1.5f;
    public Color rageColor = new Color(1f, 0.3f, 0.3f);

    [Header("Dash")]
    public float dashSpeed = 8f;
    public float dashDuration = 0.4f;
    public float dashTriggerDistance = 6f;
    public float dashChance = 0.25f;
}
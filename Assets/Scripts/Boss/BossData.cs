using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Boss/Boss Data")]
public class BossData : ScriptableObject
{
    public int MaxHealth = 20;
    public float speed = 3f;
    public float attackRange = 2f;
    public int attackDamage = 2;

    public Color hitColor = new Color(1f, 0.5f, 0.5f);

    public float attackRadius = 0.8f;
}
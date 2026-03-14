using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int MaxHealth = 3;

    public float speed = 3f;

    public float damage = 1f;

    public Color hitColor = new Color(1f, 0.5f, 0.5f);

    public float changeDirectionInterval = 2f;
}
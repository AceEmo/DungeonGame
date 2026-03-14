using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public float startHealth = 7f;
    public float maxHealth = 12f;
    public float moveSpeed = 5f;
    public float fireRate = 0.5f;
    public float bulletSpeed = 10f;
    public int damage = 1;
}
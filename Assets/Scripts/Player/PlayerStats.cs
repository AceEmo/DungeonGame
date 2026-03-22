using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Game/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public float baseStartHealth = 7f;
    public float baseMaxHealth = 12f;
    public float baseMoveSpeed = 5f;
    public float baseFireRate = 0.5f;
    public float baseBulletSpeed = 10f;
    public int baseDamage = 1;

    public float startHealth;
    public float maxHealth;
    public float moveSpeed;
    public float fireRate;
    public float bulletSpeed;
    public int damage;
    public int scrap;

    public event Action OnScrapChanged;

    private void OnEnable()
    {
        ResetAll();
    }

    public void ResetAll()
    {
        startHealth = baseStartHealth;
        maxHealth = baseMaxHealth;
        moveSpeed = baseMoveSpeed;
        fireRate = baseFireRate;
        bulletSpeed = baseBulletSpeed;
        damage = baseDamage;
        scrap = 0;
        OnScrapChanged?.Invoke();
    }

    public void AddScrap(int amount)
    {
        scrap += amount;
        OnScrapChanged?.Invoke();
    }
}
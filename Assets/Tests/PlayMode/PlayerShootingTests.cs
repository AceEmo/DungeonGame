using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerShootingTests
{
    private GameObject _playerObject;
    private GameObject _bulletPrefab;
    private PlayerShooting _shooting;
    private PlayerStats _stats;

    [SetUp]
    public void SetUp()
    {
        _playerObject = new GameObject("Player");
        _playerObject.AddComponent<AudioSource>();
        
        _bulletPrefab = new GameObject("BulletPrefab");
        _bulletPrefab.AddComponent<Rigidbody2D>();
        _bulletPrefab.AddComponent<BulletLogic>();

        _stats = ScriptableObject.CreateInstance<PlayerStats>();
        _stats.fireRate = 0.1f;
        _stats.bulletSpeed = 10f;
        _stats.damage = 2;

        _shooting = _playerObject.AddComponent<PlayerShooting>();
        typeof(PlayerShooting).GetField("stats", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_shooting, _stats);
        typeof(PlayerShooting).GetField("bulletPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_shooting, _bulletPrefab);
        typeof(PlayerShooting).GetField("firePoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(_shooting, _playerObject.transform);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_playerObject);
        Object.Destroy(_bulletPrefab);
        Object.Destroy(_stats);
    }

    [UnityTest]
    public IEnumerator Shoot_ShouldSpawnBulletWithCorrectVelocity()
    {
        _shooting.SendMessage("Start");
        yield return null;

        _shooting.SendMessage("Shoot", 1f, 0f);
        yield return null;

        GameObject spawnedBullet = GameObject.Find("BulletPrefab(Clone)");
        
        Assert.IsNotNull(spawnedBullet);
        Assert.AreEqual(new Vector2(10f, 0f), spawnedBullet.GetComponent<Rigidbody2D>().linearVelocity);

        Object.Destroy(spawnedBullet);
    }
}
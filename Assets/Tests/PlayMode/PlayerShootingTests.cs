using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerShootingTests
{
    private GameObject playerObj;
    private PlayerShooting playerShooting;
    private MockInputProvider mockInput;
    private GameObject bulletPrefab;
    private Transform firePoint;
    private GameObject audioListenerObj;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;
        
        audioListenerObj = new GameObject("AudioListener");
        audioListenerObj.AddComponent<AudioListener>();

        playerObj = new GameObject("Player");
        playerObj.SetActive(false);

        mockInput = playerObj.AddComponent<MockInputProvider>();

        firePoint = new GameObject("FirePoint").transform;
        firePoint.SetParent(playerObj.transform);
        firePoint.localPosition = Vector3.zero;

        bulletPrefab = new GameObject("Bullet");
        bulletPrefab.transform.position = new Vector3(1000f, 1000f, 0f); 
        Rigidbody2D rb = bulletPrefab.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f; 
        bulletPrefab.AddComponent<BulletLogic>();

        playerShooting = playerObj.AddComponent<PlayerShooting>();

        PlayerStats stats = ScriptableObject.CreateInstance<PlayerStats>();
        stats.fireRate = 0.5f;
        stats.bulletSpeed = 10f;
        stats.damage = 5;

        FieldInfo statsField = typeof(PlayerShooting).GetField("stats", BindingFlags.NonPublic | BindingFlags.Instance);
        statsField.SetValue(playerShooting, stats);

        FieldInfo bulletField = typeof(PlayerShooting).GetField("bulletPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        bulletField.SetValue(playerShooting, bulletPrefab);

        FieldInfo firePointField = typeof(PlayerShooting).GetField("firePoint", BindingFlags.NonPublic | BindingFlags.Instance);
        firePointField.SetValue(playerShooting, firePoint);

        FieldInfo inputField = typeof(PlayerShooting).GetField("inputProvider", BindingFlags.NonPublic | BindingFlags.Instance);
        inputField.SetValue(playerShooting, mockInput);

        playerObj.SetActive(true);
    }

    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(playerObj);
        UnityEngine.Object.DestroyImmediate(bulletPrefab);
        UnityEngine.Object.DestroyImmediate(audioListenerObj);

        foreach (var bullet in UnityEngine.Object.FindObjectsByType<BulletLogic>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            UnityEngine.Object.DestroyImmediate(bullet.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator UpdateDoesNotShootWhenNoInput()
    {
        mockInput.horizontalShoot = 0f;
        mockInput.verticalShoot = 0f;

        yield return new WaitForSeconds(0.1f);

        BulletLogic[] bullets = UnityEngine.Object.FindObjectsByType<BulletLogic>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Assert.AreEqual(1, bullets.Length); 
    }

    [UnityTest]
    public IEnumerator UpdateShootsAndInstantiatesBulletOnInput()
    {
        mockInput.horizontalShoot = 1f;
        mockInput.verticalShoot = 0f;

        yield return new WaitForSeconds(0.1f);

        BulletLogic[] bullets = UnityEngine.Object.FindObjectsByType<BulletLogic>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Assert.AreEqual(2, bullets.Length); 
    }

    [UnityTest]
    public IEnumerator UpdateRespectsFireRate()
    {
        mockInput.horizontalShoot = 1f;
        mockInput.verticalShoot = 0f;

        yield return new WaitForSeconds(0.1f);

        BulletLogic[] bullets = UnityEngine.Object.FindObjectsByType<BulletLogic>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Assert.AreEqual(2, bullets.Length); 
    }

    [UnityTest]
    public IEnumerator ShootSetsBulletVelocityAndDamage()
    {
        mockInput.horizontalShoot = 0f;
        mockInput.verticalShoot = 1f;

        yield return new WaitForSeconds(0.1f);

        BulletLogic[] bullets = UnityEngine.Object.FindObjectsByType<BulletLogic>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        BulletLogic spawnedBullet = null;
        
        foreach (var b in bullets)
        {
            if (b.gameObject != bulletPrefab)
            {
                spawnedBullet = b;
                break;
            }
        }

        Assert.IsNotNull(spawnedBullet);

        Rigidbody2D rb = spawnedBullet.GetComponent<Rigidbody2D>();
        Assert.AreEqual(new Vector2(0f, 10f), rb.linearVelocity);
        
        FieldInfo damageField = typeof(BulletLogic).GetField("damage", BindingFlags.NonPublic | BindingFlags.Instance);
        
        float damage = System.Convert.ToSingle(damageField.GetValue(spawnedBullet));
        Assert.AreEqual(5f, damage);
    }
}
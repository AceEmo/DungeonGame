using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.3f;
    private AudioSource audioSource;

    [Header("Offsets")]
    [SerializeField] private float sideOffset = 0.4f;
    [SerializeField] private float verticalOffset = 0.4f;
    [SerializeField] private float heightOffset = -0.4f;

    private float nextFireTime;
    private IInputProvider inputProvider;

    private void Start()
    {
        inputProvider = GetComponent<IInputProvider>();
        if (inputProvider == null)
        {
            inputProvider = gameObject.AddComponent<StandardInputProvider>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (inputProvider == null || stats == null || bulletPrefab == null || firePoint == null) return;

        float x = inputProvider.GetAxisRaw("HorizontalArrows");
        float y = inputProvider.GetAxisRaw("VerticalArrows");

        if ((x != 0 || y != 0) && Time.time > nextFireTime)
        {
            Shoot(x, y);
            nextFireTime = Time.time + stats.fireRate;
        }
    }

    private void Shoot(float x, float y)
    {
        Vector3 offset = Vector3.zero;
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            offset = new Vector3(sideOffset * Mathf.Sign(x), heightOffset, 0f);
        }
        else if (Mathf.Abs(y) > 0)
        {
            offset = new Vector3(0f, verticalOffset * Mathf.Sign(y), 0f);
        }

        Vector3 spawnPos = firePoint.position + offset;

        if (shootSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(shootSound, shootVolume);
        }

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = new Vector2(x, y).normalized * stats.bulletSpeed;

        BulletLogic logic = bullet.GetComponent<BulletLogic>();
        if (logic != null)
            logic.SetDamage(stats.damage);
    }
}
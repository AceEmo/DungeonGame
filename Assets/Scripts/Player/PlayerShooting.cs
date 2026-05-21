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
        InitializeComponents();
    }

    private void Update()
    {
        if (IsMissingRequiredComponents()) 
        {
            return;
        }

        float horizontalInput = inputProvider.GetAxisRaw("HorizontalArrows");
        float verticalInput = inputProvider.GetAxisRaw("VerticalArrows");

        if (CanShoot(horizontalInput, verticalInput))
        {
            Shoot(horizontalInput, verticalInput);
            nextFireTime = Time.time + stats.fireRate;
        }
    }

    private void InitializeComponents()
    {
        inputProvider = GetComponent<IInputProvider>();
        if (inputProvider == null)
        {
            inputProvider = gameObject.AddComponent<StandardInputProvider>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private bool IsMissingRequiredComponents()
    {
        return inputProvider == null || stats == null || bulletPrefab == null || firePoint == null;
    }

    private bool CanShoot(float horizontal, float vertical)
    {
        bool hasShootingInput = horizontal != 0 || vertical != 0;
        return hasShootingInput && Time.time > nextFireTime;
    }

    private void Shoot(float horizontal, float vertical)
    {
        Vector3 spawnPosition = firePoint.position + CalculateSpawnOffset(horizontal, vertical);
        PlayShootSound();

        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        SetupBulletPhysics(bullet, horizontal, vertical);
        SetupBulletLogic(bullet);
    }

    private Vector3 CalculateSpawnOffset(float horizontal, float vertical)
    {
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
        {
            return new Vector3(sideOffset * Mathf.Sign(horizontal), heightOffset, 0f);
        }
        
        if (Mathf.Abs(vertical) > 0)
        {
            return new Vector3(0f, verticalOffset * Mathf.Sign(vertical), 0f);
        }

        return Vector3.zero;
    }

    private void PlayShootSound()
    {
        if (shootSound == null)
        {
            return;
        }

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(shootSound, shootVolume);
    }

    private void SetupBulletPhysics(GameObject bullet, float horizontal, float vertical)
    {
        Rigidbody2D rigidBody = bullet.GetComponent<Rigidbody2D>();
        if (rigidBody != null)
        {
            rigidBody.linearVelocity = new Vector2(horizontal, vertical).normalized * stats.bulletSpeed;
        }
    }

    private void SetupBulletLogic(GameObject bullet)
    {
        BulletLogic logic = bullet.GetComponent<BulletLogic>();
        if (logic != null)
        {
            logic.SetDamage(stats.damage);
        }
    }
}
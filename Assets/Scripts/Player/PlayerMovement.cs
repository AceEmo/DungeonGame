using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private IInputProvider inputProvider;
    private Vector2 movement;
    private Vector2 lookDirection = new Vector2(0, -1);

    private void Awake()
    {
        inputProvider = GetComponent<IInputProvider>();
        if (inputProvider == null)
        {
            inputProvider = gameObject.AddComponent<StandardInputProvider>();
        }
    }

    private void Update()
    {
        movement.x = inputProvider.GetAxisRaw("Horizontal");
        movement.y = inputProvider.GetAxisRaw("Vertical");

        Vector2 shootingInput = new Vector2(
            inputProvider.GetAxisRaw("HorizontalArrows"),
            inputProvider.GetAxisRaw("VerticalArrows")
        );

        if (shootingInput.sqrMagnitude > 0)
            lookDirection = shootingInput;
        else if (movement.sqrMagnitude > 0)
            lookDirection = movement;

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetFloat("Horizontal", lookDirection.x);
            animator.SetFloat("Vertical", lookDirection.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    private void FixedUpdate()
    {
        if (rb != null && stats != null)
        {
            rb.MovePosition(rb.position +
                movement.normalized *
                stats.moveSpeed *
                Time.fixedDeltaTime);
        }
    }
}
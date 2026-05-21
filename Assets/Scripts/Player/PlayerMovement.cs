using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Animator animator;

    private Vector2 movement;
    private Vector2 lookDirection = new Vector2(0, -1);
    private IInputProvider inputProvider;

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
        GatherInput();
        UpdateLookDirection();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GatherInput()
    {
        movement.x = inputProvider.GetAxisRaw("Horizontal");
        movement.y = inputProvider.GetAxisRaw("Vertical");
    }

    private void UpdateLookDirection()
    {
        Vector2 shootingInput = new Vector2(
            inputProvider.GetAxisRaw("HorizontalArrows"),
            inputProvider.GetAxisRaw("VerticalArrows")
        );

        if (shootingInput.sqrMagnitude > 0)
        {
            lookDirection = shootingInput;
        }
        else if (movement.sqrMagnitude > 0)
        {
            lookDirection = movement;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            return;
        }

        animator.SetFloat("Horizontal", lookDirection.x);
        animator.SetFloat("Vertical", lookDirection.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void MovePlayer()
    {
        if (rigidBody != null && stats != null)
        {
            Vector2 targetPosition = rigidBody.position + movement.normalized * stats.moveSpeed * Time.fixedDeltaTime;
            rigidBody.MovePosition(targetPosition);
        }
    }
}
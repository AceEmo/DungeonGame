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
        if (!CanMove())
        {
            StopMovementImmediately();
            return;
        }

        GatherInput();
        UpdateLookDirection();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!CanMove())
        {
            StopPhysicsMovement();
            return;
        }

        MovePlayer();
    }

    private bool CanMove()
    {
        return GameManager.Instance != null &&
               GameManager.Instance.IsGameplayActive();
    }

    private void GatherInput()
    {
        movement.x = inputProvider.GetAxisRaw("Horizontal");
        movement.y = inputProvider.GetAxisRaw("Vertical");

        movement = Vector2.ClampMagnitude(movement, 1f);
    }

    private void UpdateLookDirection()
    {
        Vector2 shootingInput = new Vector2(
            inputProvider.GetAxisRaw("HorizontalArrows"),
            inputProvider.GetAxisRaw("VerticalArrows")
        );

        if (shootingInput.sqrMagnitude > 0.01f)
        {
            lookDirection = shootingInput.normalized;
        }
        else if (movement.sqrMagnitude > 0.01f)
        {
            lookDirection = movement.normalized;
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
        if (rigidBody == null || stats == null)
        {
            return;
        }

        Vector2 targetPosition =
            rigidBody.position +
            movement * stats.moveSpeed * Time.fixedDeltaTime;

        rigidBody.MovePosition(targetPosition);
    }

    private void StopMovementImmediately()
    {
        movement = Vector2.zero;

        StopPhysicsMovement();
        UpdateIdleAnimation();
    }

    private void StopPhysicsMovement()
    {
        if (rigidBody == null)
        {
            return;
        }

        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
    }

    private void UpdateIdleAnimation()
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            return;
        }

        animator.SetFloat("Speed", 0f);
    }
}

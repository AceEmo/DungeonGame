using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private Vector2 movement;
    private Vector2 lookDirection = new Vector2(0, -1);

    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Vector2 shootingInput = new Vector2(
            Input.GetAxisRaw("HorizontalArrows"),
            Input.GetAxisRaw("VerticalArrows")
        );

        if (shootingInput.sqrMagnitude > 0)
            lookDirection = shootingInput;
        else if (movement.sqrMagnitude > 0)
            lookDirection = movement;

        animator.SetFloat("Horizontal", lookDirection.x);
        animator.SetFloat("Vertical", lookDirection.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position +
            movement.normalized *
            stats.moveSpeed *
            Time.fixedDeltaTime);
    }
}
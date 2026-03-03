using UnityEngine;
public class Enemy : MonoBehaviour
{
    private IEnemyMovement movement;
    private IEnemyBehaviour behaviour;

    private void Awake()
    {
        movement = GetComponent<IEnemyMovement>();
        behaviour = GetComponent<IEnemyBehaviour>();
    }

    private void FixedUpdate()
    {
        Vector2 direction = behaviour.GetDirection();
        movement.Move(direction);
    }
}
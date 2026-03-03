using UnityEngine;
public class ChaseBehaviour : MonoBehaviour, IEnemyBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public Vector2 GetDirection()
    {
        if (player == null) return Vector2.zero;
        return (player.position - transform.position).normalized;
    }
}
using UnityEngine;

public class ChaseBehaviour : MonoBehaviour, IEnemyBehaviour
{
    private Transform player;

    private void Start()
    {
        TryAssignPlayer();
    }

    public Vector2 GetDirection()
    {
        if (player == null)
        {
            TryAssignPlayer();
        }

        if (player == null) return Vector2.zero;
        return (player.position - transform.position).normalized;
    }

    private void TryAssignPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject != null ? playerObject.transform : null;
    }
}
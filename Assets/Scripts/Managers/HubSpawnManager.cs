using UnityEngine;

public class HubSpawnManager : MonoBehaviour
{
    public Transform spawnPoint;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = spawnPoint.position;
        }
    }
}

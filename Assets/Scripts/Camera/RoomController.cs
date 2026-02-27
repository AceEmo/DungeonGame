using UnityEngine;
using Unity.Cinemachine;

public class RoomController : MonoBehaviour
{
    public CinemachineCamera roomCamera;
    public bool isBossRoom = false;

    private void Start()
    {
        if (isBossRoom)
        {
            SetupBossCamera();
        }

        roomCamera.Priority = 0;
    }

    private void SetupBossCamera()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        roomCamera.Follow = player.transform;
        roomCamera.LookAt = player.transform;

        var composer = roomCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
        if (composer != null)
        {
            composer.Damping = new Vector3(0f, 1f, 0f);
        }

        var confiner = roomCamera.GetComponent<CinemachineConfiner2D>();
        if (confiner != null)
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                confiner.BoundingShape2D = col;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        roomCamera.Priority = 10;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        roomCamera.Priority = 0;
    }
}
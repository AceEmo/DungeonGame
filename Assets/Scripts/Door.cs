using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject ClosedDoor;
    public GameObject OpenDoor;

    [HideInInspector] public Rooms TargetRoom;
    [HideInInspector] public Transform TargetPoint;

    private bool IsUnlocked = false;
    private bool IsTeleporting = false;

    public void Lock()
    {
        IsUnlocked = false;
        ClosedDoor.SetActive(true);
        OpenDoor.SetActive(false);
    }

    public void Unlock()
    {
        IsUnlocked = true;
        ClosedDoor.SetActive(false);
        OpenDoor.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsUnlocked || IsTeleporting) return;
        if (!other.CompareTag("Player")) return;

        Teleport(other.transform);
    }

    private void Teleport(Transform player)
    {
        if (TargetRoom == null || TargetPoint == null) return;

        IsTeleporting = true;
        player.position = TargetPoint.position;

        Invoke(nameof(ResetTeleport), 0.2f);
    }

    private void ResetTeleport()
    {
        IsTeleporting = false;
    }
}
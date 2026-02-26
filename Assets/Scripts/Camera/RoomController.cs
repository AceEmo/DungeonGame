using UnityEngine;
using Cinemachine;

public class RoomController : MonoBehaviour
{
    // Тук ще влачим камерата на стаята в Unity
    public CinemachineVirtualCamera roomCamera;

    // Когато нещо влезе в квадрата на стаята
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ако това "нещо" има таг "Player"
        if (other.CompareTag("Player"))
        {
            // Правим тази камера най-важна (Priority 10)
            roomCamera.Priority = 10;
        }
    }

    // Когато нещо излезе от квадрата
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Спираме да я ползваме (Priority 0)
            roomCamera.Priority = 0;
        }
    }
}
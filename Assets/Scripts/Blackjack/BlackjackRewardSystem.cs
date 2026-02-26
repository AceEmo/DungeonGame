using UnityEngine;
using System.Collections;

public class BlackjackRewardSystem : MonoBehaviour
{
    public GameObject rewardPrefab;
    public Transform rewardSpawnPoint;

    public IEnumerator WinRoutine(BlackjackUI ui)
    {
        yield return new WaitForSecondsRealtime(1.5f);

        if (rewardPrefab != null)
            Instantiate(rewardPrefab, rewardSpawnPoint.position, Quaternion.identity);

        ui.SetExitButton(true);
    }

    public IEnumerator LoseRoutine(BlackjackUI ui, Transform dealerTransform)
    {
        yield return new WaitForSecondsRealtime(1.5f);

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(2, dealerTransform.position);

        ui.SetExitButton(true);
    }
}
using UnityEngine;
using System.Collections;

public class BlackjackRewardSystem : MonoBehaviour
{
    public GameObject rewardPrefab;
    public GameObject blackjackRewardPrefab;

    public IEnumerator WinRoutine(BlackjackUI ui, bool isBlackjack, Transform spawnPoint)
    {
        yield return new WaitForSecondsRealtime(1.5f);

        if (spawnPoint != null)
        {
            if (isBlackjack)
            {
                if (blackjackRewardPrefab != null)
                    Instantiate(blackjackRewardPrefab, spawnPoint.position, Quaternion.identity);
            }
            else
            {
                if (rewardPrefab != null)
                    Instantiate(rewardPrefab, spawnPoint.position, Quaternion.identity);
            }
        }

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
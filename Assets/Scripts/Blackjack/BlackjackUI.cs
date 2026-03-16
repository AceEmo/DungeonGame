using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BlackjackUI : MonoBehaviour
{
    [Header("UI")]
    public Transform playerCardArea;
    public Transform dealerCardArea;
    public GameObject cardPrefab;
    public TextMeshProUGUI resultText;
    public TMPro.TextMeshProUGUI playerScoreText;
    public TMPro.TextMeshProUGUI dealerScoreText;

    public Button hitButton;
    public Button standButton;
    public Button exitButton;

    [Header("Sprites")]
    public Sprite backCardSprite;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip dealSound;
    public AudioClip flipSound;

    public GameObject SpawnCard(Sprite sprite, Transform parent)
    {
        GameObject go = Instantiate(cardPrefab, parent);

        Image img = go.GetComponent<Image>();
        img.sprite = sprite;

        go.transform.localScale = Vector3.zero;
        go.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));

        if (audioSource && dealSound)
            audioSource.PlayOneShot(dealSound);

        go.transform.DOScale(1f, 0.35f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        return go;
    }

    public void FlipCard(GameObject cardGO, Sprite newSprite)
    {
        Image img = cardGO.GetComponent<Image>();

        if (audioSource && flipSound)
            audioSource.PlayOneShot(flipSound);

        cardGO.transform
            .DORotate(new Vector3(0, 90, 0), 0.2f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                img.sprite = newSprite;
                cardGO.transform
                    .DORotate(Vector3.zero, 0.2f)
                    .SetUpdate(true);
            });
    }

    public void SetResult(string text)
    {
        resultText.text = text;
        resultText.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.4f, 10, 1).SetUpdate(true);
    }

    public void ClearTable()
    {
        foreach (Transform child in playerCardArea)
            Destroy(child.gameObject);

        foreach (Transform child in dealerCardArea)
            Destroy(child.gameObject);
    }

    public void EnableButtons(bool state)
    {
        hitButton.interactable = state;
        standButton.interactable = state;
    }
    public void SetExitButton(bool state)
    {
        exitButton.interactable = state;
    }

    public void UpdateScores(int playerScore, int dealerScore)
    {
        if (playerScoreText != null)
            playerScoreText.text = playerScore.ToString();

        if (dealerScoreText != null)
            dealerScoreText.text = dealerScore.ToString();
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BlackjackUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform playerCardArea;
    [SerializeField] private Transform dealerCardArea;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI dealerScoreText;

    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button exitButton;

    [Header("Sprites")]
    [SerializeField] private Sprite backCardSprite;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dealSound;
    [SerializeField] private AudioClip flipSound;

    public Transform PlayerCardArea => playerCardArea;
    public Transform DealerCardArea => dealerCardArea;
    public Sprite BackCardSprite => backCardSprite;

    public GameObject SpawnCard(Sprite sprite, Transform parent)
    {
        if (cardPrefab == null || parent == null) return null;

        GameObject go = Instantiate(cardPrefab, parent);

        Image img = go.GetComponent<Image>();
        if (img == null) return go;

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
        if (cardGO == null || newSprite == null) return;

        Image img = cardGO.GetComponent<Image>();
        if (img == null) return;

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
        if (resultText == null) return;

        resultText.text = text;
        resultText.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.4f, 10, 1).SetUpdate(true);
    }

    public void ClearTable()
    {
        if (playerCardArea != null)
        {
            foreach (Transform child in playerCardArea)
                Destroy(child.gameObject);
        }

        if (dealerCardArea != null)
        {
            foreach (Transform child in dealerCardArea)
                Destroy(child.gameObject);
        }
    }

    public void EnableButtons(bool state)
    {
        if (hitButton != null) hitButton.interactable = state;
        if (standButton != null) standButton.interactable = state;
    }
    public void SetExitButton(bool state)
    {
        if (exitButton != null) exitButton.interactable = state;
    }

    public void UpdateScores(int playerScore, int dealerScore)
    {
        if (playerScoreText != null)
            playerScoreText.text = playerScore.ToString();

        if (dealerScoreText != null)
            dealerScoreText.text = dealerScore.ToString();
    }
}
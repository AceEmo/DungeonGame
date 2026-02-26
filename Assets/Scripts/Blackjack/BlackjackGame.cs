using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class BlackjackGame : MonoBehaviour
{
    [Header("UI")]
    public Transform playerCardArea;
    public Transform dealerCardArea;
    public GameObject cardPrefab;
    public TextMeshProUGUI resultText;

    public Button hitButton;
    public Button standButton;
    public Button exitButton;
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip dealSound;
    public AudioClip flipSound;
    [Header("Root Canvas")]
    public GameObject blackjackRoot;

    [Header("Sprites")]
    public Sprite[] cardSprites;
    public Sprite backCardSprite;

    [Header("Reward")]
    public GameObject rewardPrefab;
    public Transform rewardSpawnPoint;

    private List<Card> deck = new List<Card>();
    private List<Card> dealerCards = new List<Card>();
    private List<Card> playerCards = new List<Card>();

    private int playerScore;
    private int dealerScore;

    private bool gameStarted;
    private bool gameOver;
    private bool dealerTurn;

    private GameObject hiddenDealerCardGO;

    public void StartBlackjack()
    {
        StartCoroutine(GameStartSequence());
    }

    IEnumerator GameStartSequence()
    {
        if (gameStarted) yield break;
        PrepareGame();
        if (!gameStarted) yield break;

        DisableButtons();

        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(DealInitialCards());
        EnableButtons();
    }

    void PrepareGame()
    {
        if (playerCardArea == null || dealerCardArea == null || cardPrefab == null)
        {
            Debug.LogError("BlackjackGame: UI references missing.");
            gameStarted = false;
            return;
        }

        if (resultText == null || hitButton == null || standButton == null || exitButton == null)
        {
            Debug.LogError("BlackjackGame: Buttons or resultText missing.");
            gameStarted = false;
            return;
        }

        if (cardSprites == null || cardSprites.Length == 0)
        {
            Debug.LogError("BlackjackGame: No card sprites assigned.");
            gameStarted = false;
            return;
        }

        if (backCardSprite == null)
        {
            Debug.LogError("BlackjackGame: backCardSprite missing.");
            gameStarted = false;
            return;
        }

        ClearTable();
        SetupDeck();

        if (deck.Count == 0)
        {
            Debug.LogError("BlackjackGame: Deck empty after setup.");
            gameStarted = false;
            return;
        }

        playerCards.Clear();
        dealerCards.Clear();

        playerScore = 0;
        dealerScore = 0;

        gameOver = false;
        dealerTurn = false;
        gameStarted = true;

        resultText.text = "";
        exitButton.interactable = false;
        gameStarted = true;
    }

    // ---------------------------------------------------------
    //  DECK SETUP
    // ---------------------------------------------------------
    void SetupDeck()
    {
        deck.Clear();

        foreach (Sprite s in cardSprites)
        {
            if (s == null) continue;
            if (s.name == "cardBackRed") continue;

            int value = GetCardValueFromName(s.name);
            deck.Add(new Card(s.name, value, s));
        }
    }

    int GetCardValueFromName(string name)
    {
        int underscoreIndex = name.LastIndexOf('_');
        string rank = name.Substring(underscoreIndex + 1);

        if (rank == "A") return 11;
        if (rank == "K" || rank == "Q" || rank == "J") return 10;

        int parsed;
        if (!int.TryParse(rank, out parsed))
            return 0;

        return parsed;
    }

    // ---------------------------------------------------------
    //  INITIAL DEAL
    // ---------------------------------------------------------
    IEnumerator DealInitialCards()
    {
        DrawCardToPlayer();
        yield return new WaitForSecondsRealtime(0.4f);

        DrawCardToDealer(true);
        yield return new WaitForSecondsRealtime(0.4f);

        DrawCardToPlayer();
        yield return new WaitForSecondsRealtime(0.4f);

        DrawCardToDealer(false);
        yield return new WaitForSecondsRealtime(0.4f);

        if (playerScore == 21 && playerCards.Count == 2)
        {
            RevealDealerCard();

            resultText.text = "BLACKJACK!";
            resultText.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.4f, 10, 1).SetUpdate(true);

            StartCoroutine(EndWin());
        }
    }


    // ---------------------------------------------------------
    //  DRAW CARDS
    // ---------------------------------------------------------
    void DrawCardToPlayer()
    {
        Card card = DrawCard();
        if (card == null) return;

        playerCards.Add(card);
        SpawnCard(card.sprite, playerCardArea);

        playerScore = CalculateScore(playerCards);
    }

    void DrawCardToDealer(bool hidden)
    {
        Card card = DrawCard();
        if (card == null) return;

        dealerCards.Add(card);

        GameObject go = SpawnCard(hidden ? backCardSprite : card.sprite, dealerCardArea);

        if (hidden)
            hiddenDealerCardGO = go;

        if (!hidden)
            dealerScore = CalculateScore(dealerCards);
    }

    Card DrawCard()
    {
        if (deck.Count == 0) return null;

        int index = Random.Range(0, deck.Count);
        Card card = deck[index];
        deck.RemoveAt(index);
        return card;
    }

    // ---------------------------------------------------------
    //  SPAWN CARD WITH DOTWEEN ANIMATION
    // ---------------------------------------------------------
    GameObject SpawnCard(Sprite sprite, Transform parent)
    {
        GameObject go = Instantiate(cardPrefab, parent);

        Image img = go.GetComponent<Image>();
        img.sprite = sprite;

        go.transform.localScale = Vector3.zero;
        go.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(-20f, 20f));

        float volume = (playerCards.Count + dealerCards.Count < 4) ? 0.25f : 1f;

        if (audioSource && dealSound)
            audioSource.PlayOneShot(dealSound, volume);

        go.transform.DOScale(1f, 0.35f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        return go;
    }

    // ---------------------------------------------------------
    //  ACE LOGIC
    // ---------------------------------------------------------
    int CalculateScore(List<Card> cards)
    {
        int total = 0;
        int aces = 0;

        foreach (Card c in cards)
        {
            total += c.value;
            if (c.value == 11) aces++;
        }

        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }

        return total;
    }

    // ---------------------------------------------------------
    //  PLAYER ACTIONS
    // ---------------------------------------------------------
    public void Hit()
    {
        if (!gameStarted || gameOver || dealerTurn) return;

        DrawCardToPlayer();

        if (playerScore > 21)
        {
            StartCoroutine(PlayerBust());
            return;
        }

        // Ако играчът направи точно 21 с повече от 2 карти → автоматичен Stand
        if (playerScore == 21 && playerCards.Count > 2)
        {
            StartCoroutine(DealerTurn());
            return;
        }
    }


    IEnumerator PlayerBust()
    {
        gameOver = true;
        DisableButtons();

        resultText.text = "YOU LOSE!";

        yield return new WaitForSecondsRealtime(1f);

        RevealDealerCard();

        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(EndLose());
    }

    public void Stand()
    {
        if (!gameStarted || gameOver || dealerTurn) return;

        StartCoroutine(DealerTurn());
    }

    // ---------------------------------------------------------
    //  DEALER TURN
    // ---------------------------------------------------------
    IEnumerator DealerTurn()
    {
        dealerTurn = true;
        DisableButtons();

        RevealDealerCard();
        yield return new WaitForSecondsRealtime(1f);

        while (dealerScore < 17)
        {
            DrawCardToDealer(false);
            yield return new WaitForSecondsRealtime(0.8f);
        }

        DetermineWinner();
    }

    // ---------------------------------------------------------
    //  FLIP ANIMATION
    // ---------------------------------------------------------
    void RevealDealerCard()
    {
        if (dealerCards.Count == 0 || hiddenDealerCardGO == null) return;

        Card hiddenCard = dealerCards[0];
        Image img = hiddenDealerCardGO.GetComponent<Image>();

        // 🔊 Flip sound
        if (audioSource && flipSound)
            audioSource.PlayOneShot(flipSound);

        hiddenDealerCardGO.transform
            .DORotate(new Vector3(0, 90, 0), 0.2f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                img.sprite = hiddenCard.sprite;
                hiddenDealerCardGO.transform
                    .DORotate(Vector3.zero, 0.2f)
                    .SetUpdate(true);
            });

        dealerScore = CalculateScore(dealerCards);
    }
    // ---------------------------------------------------------
    //  END GAME LOGIC
    // ---------------------------------------------------------
    void DetermineWinner()
    {
        gameOver = true;

        if (dealerScore > 21)
            Win();
        else if (playerScore > dealerScore)
            Win();
        else if (playerScore < dealerScore)
            Lose();
        else
            Push();
    }

    void Win()
    {
        resultText.text = "YOU WIN!";
        resultText.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.4f, 10, 1).SetUpdate(true);

        StartCoroutine(EndWin());
    }


    void Lose()
    {
        resultText.text = "YOU LOSE!";
        StartCoroutine(EndLose());
    }

    void Push()
    {
        resultText.text = "PUSH!";
        StartCoroutine(EndPush());
    }

    IEnumerator EndWin()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        if (rewardPrefab != null)
            Instantiate(rewardPrefab, rewardSpawnPoint.position, Quaternion.identity);

        EnableExitButton();
    }

    IEnumerator EndLose()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(2, transform.position);

        EnableExitButton();
    }

    IEnumerator EndPush()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        EnableExitButton();
    }

    void EnableExitButton()
    {
        exitButton.interactable = true;
    }

    public void ExitGame()
    {
        if (!gameOver) return;

        Time.timeScale = 1f;

        if (blackjackRoot != null)
            blackjackRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    // ---------------------------------------------------------
    //  UI HELPERS
    // ---------------------------------------------------------
    void DisableButtons()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
    }

    void EnableButtons()
    {
        hitButton.interactable = true;
        standButton.interactable = true;
    }

    void ClearTable()
    {
        foreach (Transform child in playerCardArea)
            Destroy(child.gameObject);

        foreach (Transform child in dealerCardArea)
            Destroy(child.gameObject);
    }
}
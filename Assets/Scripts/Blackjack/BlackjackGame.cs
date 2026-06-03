using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BlackjackUI))]
[RequireComponent(typeof(BlackjackRewardSystem))]
public class BlackjackGame : MonoBehaviour
{
    public Sprite[] cardSprites;

    private BlackjackDeck deck;
    private BlackjackHand playerHand;
    private BlackjackHand dealerHand;

    private BlackjackUI ui;
    private BlackjackRewardSystem rewardSystem;

    private GameObject hiddenDealerCard;

    private bool gameOver;
    private bool rewardProcessing;
    private bool dealerCardHidden;
    private bool playerActionLocked;

    private Transform itemSpawnPoint;

    private Coroutine gameFlowCoroutine;

    private void Awake()
    {
        deck = new BlackjackDeck();
        playerHand = new BlackjackHand();
        dealerHand = new BlackjackHand();

        ui = GetComponent<BlackjackUI>();
        rewardSystem = GetComponent<BlackjackRewardSystem>();

        if (ui == null || rewardSystem == null)
        {
            Debug.LogError($"{nameof(BlackjackGame)} on {name} requires {nameof(BlackjackUI)} and {nameof(BlackjackRewardSystem)}.", this);
            enabled = false;
        }
    }

    public void SetItemSpawnPoint(Transform point)
    {
        itemSpawnPoint = point;
    }

    public void StartBlackjack()
    {
        if (ui == null || rewardSystem == null) return;

        ui.SetResult(" ");

        if (gameFlowCoroutine != null)
        {
            StopCoroutine(gameFlowCoroutine);
        }

        gameFlowCoroutine = StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        ResetRoundState();
        ui.SetExitButton(false);
        ui.ClearTable();
        ui.EnableButtons(false);
        ui.UpdateScores(0, 0);

        deck.Setup(cardSprites);

        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(InitialDeal());

        CheckForInitialBlackjack();

        if (!gameOver)
        {
            ui.EnableButtons(true);
        }

        gameFlowCoroutine = null;
    }

    private void ResetRoundState()
    {
        playerHand.Clear();
        dealerHand.Clear();
        gameOver = false;
        rewardProcessing = false;
        dealerCardHidden = false;
        playerActionLocked = false;
    }

    private void UpdateAllScores()
    {
        int playerScore = playerHand.GetScore();
        int dealerScore = dealerCardHidden
            ? dealerHand.GetScoreWithoutFirstCard()
            : dealerHand.GetScore();

        ui.UpdateScores(playerScore, dealerScore);
    }

    IEnumerator InitialDeal()
    {
        yield return DealToPlayer();
        yield return DealToDealer(true);
        yield return DealToPlayer();
        yield return DealToDealer(false);
    }

    IEnumerator DealToPlayer()
    {
        Card card = deck.Draw();
        playerHand.AddCard(card);
        ui.SpawnCard(card.sprite, ui.PlayerCardArea);
        UpdateAllScores();
        yield return new WaitForSecondsRealtime(0.4f);
    }

    IEnumerator DealToDealer(bool hidden)
    {
        Card card = deck.Draw();
        dealerHand.AddCard(card);

        GameObject cardObject = ui.SpawnCard(hidden ? ui.BackCardSprite : card.sprite, ui.DealerCardArea);
        if (hidden)
        {
            hiddenDealerCard = cardObject;
            dealerCardHidden = true;
        }

        UpdateAllScores();
        yield return new WaitForSecondsRealtime(0.4f);
    }

    void CheckForInitialBlackjack()
    {
        bool playerBJ = playerHand.HasBlackjack();
        bool dealerBJ = dealerHand.HasBlackjack();

        if (!playerBJ && !dealerBJ)
        {
            return;
        }

        gameOver = true;
        ui.EnableButtons(false);
        StartCoroutine(InitialBlackjackFlow(playerBJ, dealerBJ));
    }

    IEnumerator InitialBlackjackFlow(bool playerBJ, bool dealerBJ)
    {
        yield return RevealDealerCard();

        rewardProcessing = true;

        if (playerBJ && dealerBJ)
        {
            ui.SetResult("DRAW!");
        }
        else if (playerBJ)
        {
            ui.SetResult("BLACKJACK!");
            yield return rewardSystem.WinRoutine(ui, true, itemSpawnPoint);
        }
        else
        {
            ui.SetResult("YOU LOSE!");
            yield return rewardSystem.LoseRoutine(ui, transform);
        }

        yield return FinishRound();
    }

    public void Hit()
    {
        if (gameOver || rewardProcessing || playerActionLocked) return;

        playerActionLocked = true;
        StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        yield return DealToPlayer();

        int playerScore = playerHand.GetScore();
        UpdateAllScores();

        if (playerScore > 21)
        {
            yield return HandlePlayerBust();
        }
        else if (playerScore == 21)
        {
            yield return StartCoroutine(DealerTurn());
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.2f);
            playerActionLocked = false;
        }
    }

    public void Stand()
    {
        if (gameOver || rewardProcessing || playerActionLocked) return;

        playerActionLocked = true;
        StartCoroutine(DealerTurn());
    }

    IEnumerator DealerTurn()
    {
        ui.EnableButtons(false);
        yield return RevealDealerCard();

        while (dealerHand.GetScore() < 17)
        {
            yield return DealToDealer(false);
        }

        yield return DetermineWinner();
    }

    IEnumerator DetermineWinner()
    {
        gameOver = true;
        rewardProcessing = true;
        UpdateAllScores();

        int playerScore = playerHand.GetScore();
        int dealerScore = dealerHand.GetScore();

        if (dealerScore > 21 || playerScore > dealerScore)
        {
            ui.SetResult("YOU WIN!");
            yield return rewardSystem.WinRoutine(ui, false, itemSpawnPoint);
        }
        else if (playerScore < dealerScore)
        {
            ui.SetResult("YOU LOSE!");
            yield return rewardSystem.LoseRoutine(ui, transform);
        }
        else
        {
            ui.SetResult("DRAW!");
        }

        yield return FinishRound();
    }

    public void ExitGame()
    {
        if (!gameOver || rewardProcessing) return;

        if (ui != null)
        {
            ui.SetResult(" ");
            ui.ClearTable();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CloseBlackjack();
        }
        else
        {
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private IEnumerator RevealDealerCard()
    {
        ui.FlipCard(hiddenDealerCard, dealerHand.Cards[0].sprite);
        dealerCardHidden = false;
        UpdateAllScores();
        yield return new WaitForSecondsRealtime(1f);
    }

    private IEnumerator HandlePlayerBust()
    {
        gameOver = true;
        ui.EnableButtons(false);
        yield return RevealDealerCard();

        ui.SetResult("YOU LOSE!");
        rewardProcessing = true;
        yield return rewardSystem.LoseRoutine(ui, transform);
        yield return FinishRound();
    }

    private IEnumerator FinishRound()
    {
        rewardProcessing = false;
        playerActionLocked = false;
        ui.SetExitButton(true);
        yield break;
    }
}

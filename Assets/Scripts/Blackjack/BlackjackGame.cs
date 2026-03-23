using UnityEngine;
using System.Collections;

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

    private Transform itemSpawnPoint;

    private bool hitLocked = false;

    private void Awake()
    {
        deck = new BlackjackDeck();
        playerHand = new BlackjackHand();
        dealerHand = new BlackjackHand();

        ui = GetComponent<BlackjackUI>();
        rewardSystem = GetComponent<BlackjackRewardSystem>();
    }

    public void SetItemSpawnPoint(Transform point)
    {
        itemSpawnPoint = point;
    }

    public void StartBlackjack()
    {
        if (ui != null)
            ui.SetResult(" ");

        StartCoroutine(GameFlow());
    }

    IEnumerator GameFlow()
    {
        ui.SetExitButton(false);
        ui.ClearTable();

        deck.Setup(cardSprites);

        playerHand.Clear();
        dealerHand.Clear();

        gameOver = false;
        rewardProcessing = false;
        dealerCardHidden = false;
        hitLocked = false;

        ui.EnableButtons(false);

        ui.UpdateScores(0, 0);

        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(InitialDeal());

        CheckForInitialBlackjack();

        if (!gameOver)
            ui.EnableButtons(true);
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

        ui.SpawnCard(card.sprite, ui.playerCardArea);

        UpdateAllScores();

        yield return new WaitForSecondsRealtime(0.4f);
    }

    IEnumerator DealToDealer(bool hidden)
    {
        Card card = deck.Draw();
        dealerHand.AddCard(card);

        GameObject go = ui.SpawnCard(hidden ? ui.backCardSprite : card.sprite, ui.dealerCardArea);

        if (hidden)
        {
            hiddenDealerCard = go;
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
            return;

        gameOver = true;
        ui.EnableButtons(false);

        StartCoroutine(InitialBlackjackFlow(playerBJ, dealerBJ));
    }

    IEnumerator InitialBlackjackFlow(bool playerBJ, bool dealerBJ)
    {
        ui.FlipCard(hiddenDealerCard, dealerHand.Cards[0].sprite);
        dealerCardHidden = false;
        UpdateAllScores();            

        yield return new WaitForSecondsRealtime(1f);

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

        rewardProcessing = false;
        ui.SetExitButton(true);
    }

    public void Hit()
    {
        if (gameOver || rewardProcessing || hitLocked) return;

        hitLocked = true;
        StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        yield return DealToPlayer();

        int playerScore = playerHand.GetScore();

        UpdateAllScores();

        if (playerScore > 21)
        {
            gameOver = true;
            ui.EnableButtons(false);

            ui.FlipCard(hiddenDealerCard, dealerHand.Cards[0].sprite);
            dealerCardHidden = false;   
            UpdateAllScores();          

            yield return new WaitForSecondsRealtime(1f);

            ui.SetResult("YOU LOSE!");

            rewardProcessing = true;
            yield return rewardSystem.LoseRoutine(ui, transform);
            rewardProcessing = false;

            ui.SetExitButton(true);
        }
        else if (playerScore == 21)
        {
            yield return StartCoroutine(DealerTurn());
        }

        yield return new WaitForSecondsRealtime(0.2f);
        hitLocked = false;
    }

    public void Stand()
    {
        if (gameOver || rewardProcessing) return;

        StartCoroutine(DealerTurn());
    }

    IEnumerator DealerTurn()
    {
        ui.EnableButtons(false);

        ui.FlipCard(hiddenDealerCard, dealerHand.Cards[0].sprite);
        dealerCardHidden = false;
        UpdateAllScores();          

        yield return new WaitForSecondsRealtime(1f);

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

        rewardProcessing = false;
        ui.SetExitButton(true);
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
}
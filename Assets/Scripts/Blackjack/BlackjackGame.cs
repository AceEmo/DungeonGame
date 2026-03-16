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

    private void Awake()
    {
        deck = new BlackjackDeck();
        playerHand = new BlackjackHand();
        dealerHand = new BlackjackHand();

        ui = GetComponent<BlackjackUI>();
        rewardSystem = GetComponent<BlackjackRewardSystem>();
    }

    public void StartBlackjack()
    {
        if (ui != null)
            ui.SetResult("");
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

        ui.EnableButtons(false);

        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(InitialDeal());

        CheckForInitialBlackjack();

        if (!gameOver)
            ui.EnableButtons(true);
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
        yield return new WaitForSecondsRealtime(0.4f);
    }

    IEnumerator DealToDealer(bool hidden)
    {
        Card card = deck.Draw();
        dealerHand.AddCard(card);

        GameObject go = ui.SpawnCard(hidden ? ui.backCardSprite : card.sprite, ui.dealerCardArea);

        if (hidden)
            hiddenDealerCard = go;

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
        yield return new WaitForSecondsRealtime(1f);

        rewardProcessing = true;

        if (playerBJ && dealerBJ)
        {
            ui.SetResult("DRAW!");
        }
        else if (playerBJ)
        {
            ui.SetResult("BLACKJACK!");
            yield return rewardSystem.WinRoutine(ui);
        }
        else
        {
            ui.SetResult("YOU LOSE!");
            yield return rewardSystem.LoseRoutine(ui, transform);
        }

        rewardProcessing = false;
        ui.SetExitButton(true);
    }

    private bool hitLocked = false;

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

        if (playerScore > 21)
        {
            gameOver = true;
            ui.EnableButtons(false);

            ui.FlipCard(hiddenDealerCard, dealerHand.Cards[0].sprite);
            yield return new WaitForSecondsRealtime(1f);

            ui.SetResult("YOU LOSE!");

            rewardProcessing = true;
            yield return rewardSystem.LoseRoutine(ui, transform);
            rewardProcessing = false;

            ui.SetExitButton(true);
        }
        else if (playerScore == 21)
        {
            StartCoroutine(DealerTurn());
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

        int playerScore = playerHand.GetScore();
        int dealerScore = dealerHand.GetScore();

        if (dealerScore > 21 || playerScore > dealerScore)
        {
            ui.SetResult("YOU WIN!");
            yield return rewardSystem.WinRoutine(ui);
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

        Time.timeScale = 1f;
        gameObject.SetActive(false);

        if (ui != null)
        {
            ui.SetResult("");
            ui.ClearTable();
        }
    }
}
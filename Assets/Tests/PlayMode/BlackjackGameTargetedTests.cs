using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class BlackjackGameTargetedTests
{
    private GameObject go;
    private BlackjackGame game;
    private BlackjackUI ui;
    private BlackjackRewardSystem reward;
    private Sprite dummySprite;

    [SetUp]
    public void Setup()
    {
        Time.timeScale = 1f;

        go = new GameObject("BlackjackSystem");

        ui = go.AddComponent<BlackjackUI>();
        ui.playerCardArea = new GameObject("PlayerArea").transform;
        ui.dealerCardArea = new GameObject("DealerArea").transform;
        ui.cardPrefab = new GameObject("CardPrefab");
        ui.cardPrefab.AddComponent<Image>();
        ui.resultText = new GameObject("ResultText").AddComponent<TextMeshProUGUI>();
        ui.playerScoreText = new GameObject("PlayerScore").AddComponent<TextMeshProUGUI>();
        ui.dealerScoreText = new GameObject("DealerScore").AddComponent<TextMeshProUGUI>();
        ui.hitButton = new GameObject("HitBtn").AddComponent<Button>();
        ui.standButton = new GameObject("StandBtn").AddComponent<Button>();
        ui.exitButton = new GameObject("ExitBtn").AddComponent<Button>();

        Texture2D tex = new Texture2D(2, 2);
        dummySprite = Sprite.Create(tex, new Rect(0, 0, 2, 2), Vector2.zero);
        ui.backCardSprite = dummySprite;

        reward = go.AddComponent<BlackjackRewardSystem>();
        reward.rewardSpawnPoint = new GameObject("Spawn").transform;
        
        game = go.AddComponent<BlackjackGame>();
        game.cardSprites = new Sprite[] { dummySprite };
    }

    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.DestroyImmediate(go);
        if (dummySprite != null) UnityEngine.Object.DestroyImmediate(dummySprite.texture);
    }

    private BlackjackHand GetHand(string handName)
    {
        FieldInfo field = typeof(BlackjackGame).GetField(handName, BindingFlags.NonPublic | BindingFlags.Instance);
        return (BlackjackHand)field.GetValue(game);
    }

    private void SetRiggedDeck(int value, int count)
    {
        BlackjackDeck deck = new BlackjackDeck();
        FieldInfo deckList = typeof(BlackjackDeck).GetField("deck", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Card> list = new List<Card>();
        for(int i = 0; i < count; i++) list.Add(new Card("rigged", value, dummySprite));
        deckList.SetValue(deck, list);

        FieldInfo gameDeck = typeof(BlackjackGame).GetField("deck", BindingFlags.NonPublic | BindingFlags.Instance);
        gameDeck.SetValue(game, deck);
    }

    private IEnumerator RunDetermineWinner()
    {
        MethodInfo method = typeof(BlackjackGame).GetMethod("DetermineWinner", BindingFlags.NonPublic | BindingFlags.Instance);
        yield return game.StartCoroutine((IEnumerator)method.Invoke(game, null));
    }

    private void SetupHiddenCardForTest()
    {
        FieldInfo hiddenCardField = typeof(BlackjackGame).GetField("hiddenDealerCard", BindingFlags.NonPublic | BindingFlags.Instance);
        GameObject dummyCardGo = new GameObject("HiddenCard");
        dummyCardGo.AddComponent<Image>();
        hiddenCardField.SetValue(game, dummyCardGo);
    }

    [UnityTest]
    public IEnumerator DetermineWinner_PlayerWins_SetsUI()
    {
        GetHand("playerHand").AddCard(new Card("p", 20, dummySprite));
        GetHand("dealerHand").AddCard(new Card("d", 18, dummySprite));

        yield return RunDetermineWinner();

        Assert.AreEqual("YOU WIN!", ui.resultText.text);
    }

    [UnityTest]
    public IEnumerator DetermineWinner_DealerBusts_PlayerWins()
    {
        GetHand("playerHand").AddCard(new Card("p", 15, dummySprite));
        GetHand("dealerHand").AddCard(new Card("d", 22, dummySprite));

        yield return RunDetermineWinner();

        Assert.AreEqual("YOU WIN!", ui.resultText.text);
    }

    [UnityTest]
    public IEnumerator DetermineWinner_DealerWins_SetsUI()
    {
        GetHand("playerHand").AddCard(new Card("p", 17, dummySprite));
        GetHand("dealerHand").AddCard(new Card("d", 19, dummySprite));

        yield return RunDetermineWinner();

        Assert.AreEqual("YOU LOSE!", ui.resultText.text);
    }

    [UnityTest]
    public IEnumerator DetermineWinner_Draw_SetsUI()
    {
        GetHand("playerHand").AddCard(new Card("p", 18, dummySprite));
        GetHand("dealerHand").AddCard(new Card("d", 18, dummySprite));

        yield return RunDetermineWinner();

        Assert.AreEqual("DRAW!", ui.resultText.text);
    }

    [UnityTest]
    public IEnumerator HitRoutine_BustsPlayer_EndsGameAndSetsLose()
    {
        SetRiggedDeck(10, 1);
        GetHand("playerHand").AddCard(new Card("p", 15, dummySprite));
        GetHand("dealerHand").AddCard(new Card("d", 10, dummySprite));
        SetupHiddenCardForTest();

        MethodInfo hitRoutine = typeof(BlackjackGame).GetMethod("HitRoutine", BindingFlags.NonPublic | BindingFlags.Instance);
        yield return game.StartCoroutine((IEnumerator)hitRoutine.Invoke(game, null));

        Assert.AreEqual("YOU LOSE!", ui.resultText.text);
        
        FieldInfo gameOverField = typeof(BlackjackGame).GetField("gameOver", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsTrue((bool)gameOverField.GetValue(game));
    }

    [UnityTest]
    public IEnumerator HitRoutine_PlayerHits21_AutoStartsDealerTurn()
    {
        SetRiggedDeck(10, 5); 
        GetHand("playerHand").AddCard(new Card("p", 11, dummySprite));
        GetHand("dealerHand").AddCard(new Card("d", 5, dummySprite));
        SetupHiddenCardForTest();

        MethodInfo hitRoutine = typeof(BlackjackGame).GetMethod("HitRoutine", BindingFlags.NonPublic | BindingFlags.Instance);
        yield return game.StartCoroutine((IEnumerator)hitRoutine.Invoke(game, null));

        Assert.AreEqual("YOU WIN!", ui.resultText.text);
    }

    [UnityTest]
    public IEnumerator DealerTurn_DrawsUntil17()
    {
        SetRiggedDeck(5, 10); 
        GetHand("dealerHand").AddCard(new Card("d", 10, dummySprite));
        GetHand("playerHand").AddCard(new Card("p", 20, dummySprite));
        SetupHiddenCardForTest();

        MethodInfo dealerTurn = typeof(BlackjackGame).GetMethod("DealerTurn", BindingFlags.NonPublic | BindingFlags.Instance);
        yield return game.StartCoroutine((IEnumerator)dealerTurn.Invoke(game, null));

        Assert.AreEqual(20, GetHand("dealerHand").GetScore());
        Assert.AreEqual("DRAW!", ui.resultText.text);
    }

    [UnityTest]
    public IEnumerator InitialBlackjackFlow_PlayerBJ_Wins()
    {
        SetupHiddenCardForTest();
        GetHand("dealerHand").AddCard(new Card("d", 10, dummySprite)); 

        MethodInfo flow = typeof(BlackjackGame).GetMethod("InitialBlackjackFlow", BindingFlags.NonPublic | BindingFlags.Instance);
        yield return game.StartCoroutine((IEnumerator)flow.Invoke(game, new object[] { true, false }));

        Assert.AreEqual("BLACKJACK!", ui.resultText.text);
    }

    [UnityTest]
    public IEnumerator InitialBlackjackFlow_BothBJ_Draw()
    {
        SetupHiddenCardForTest();
        GetHand("dealerHand").AddCard(new Card("d", 10, dummySprite)); 

        MethodInfo flow = typeof(BlackjackGame).GetMethod("InitialBlackjackFlow", BindingFlags.NonPublic | BindingFlags.Instance);
        yield return game.StartCoroutine((IEnumerator)flow.Invoke(game, new object[] { true, true }));

        Assert.AreEqual("DRAW!", ui.resultText.text);
    }
}
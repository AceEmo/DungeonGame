using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

public class BlackjackGameTests
{
    private GameObject _gameGameObject;
    private BlackjackGame _blackjackGame;
    private BlackjackUI _ui;
    private BlackjackRewardSystem _rewardSystem;

    [SetUp]
    public void SetUp()
    {
        _gameGameObject = new GameObject("BlackjackGame");
        
        _ui = _gameGameObject.AddComponent<BlackjackUI>();
        SetupMockUI(_ui);

        _rewardSystem = _gameGameObject.AddComponent<BlackjackRewardSystem>();
        _blackjackGame = _gameGameObject.AddComponent<BlackjackGame>();

        _blackjackGame.cardSprites = new Sprite[] 
        { 
            CreateMockSprite("hearts_10"), 
            CreateMockSprite("diamonds_2"), 
            CreateMockSprite("clubs_3"), 
            CreateMockSprite("spades_4"),
            CreateMockSprite("hearts_5"),
            CreateMockSprite("diamonds_6")
        };
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameGameObject);
    }

    private void SetupMockUI(BlackjackUI ui)
    {
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
        
        ui.backCardSprite = CreateMockSprite("back");
    }

    private Sprite CreateMockSprite(string name)
    {
        Sprite sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        sprite.name = name;
        return sprite;
    }

    [UnityTest]
    public IEnumerator StartBlackjack_ShouldClearTableAndSetupGame()
    {
        _blackjackGame.StartBlackjack();

        yield return new WaitForSeconds(1.5f);

        Assert.AreEqual(" ", _ui.resultText.text);
        Assert.AreEqual(1, _ui.playerCardArea.childCount);
        Assert.AreEqual(1, _ui.dealerCardArea.childCount);
    }

    [UnityTest]
    public IEnumerator Hit_WhenGameIsActive_ShouldDealCardToPlayer()
    {
        _blackjackGame.StartBlackjack();
        yield return new WaitForSeconds(1.5f);

        int initialCardCount = _ui.playerCardArea.childCount;

        _blackjackGame.Hit();
        yield return new WaitForSeconds(0.5f);

        Assert.Greater(_ui.playerCardArea.childCount, initialCardCount);
    }
}
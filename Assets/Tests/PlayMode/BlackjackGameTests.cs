using System.Collections;
using System.Reflection; // Добавено за Reflection
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
        var playerArea = new GameObject("PlayerArea").transform;
        var dealerArea = new GameObject("DealerArea").transform;
        
        var cardPrefab = new GameObject("CardPrefab");
        cardPrefab.AddComponent<Image>();

        var resultText = new GameObject("ResultText").AddComponent<TextMeshProUGUI>();
        var playerScoreText = new GameObject("PlayerScore").AddComponent<TextMeshProUGUI>();
        var dealerScoreText = new GameObject("DealerScore").AddComponent<TextMeshProUGUI>();

        var hitButton = new GameObject("HitBtn").AddComponent<Button>();
        var standButton = new GameObject("StandBtn").AddComponent<Button>();
        var exitButton = new GameObject("ExitBtn").AddComponent<Button>();
        
        var backCardSprite = CreateMockSprite("back");

        SetPrivateField(ui, "playerCardArea", playerArea);
        SetPrivateField(ui, "dealerCardArea", dealerArea);
        SetPrivateField(ui, "cardPrefab", cardPrefab);
        SetPrivateField(ui, "resultText", resultText);
        SetPrivateField(ui, "playerScoreText", playerScoreText);
        SetPrivateField(ui, "dealerScoreText", dealerScoreText);
        SetPrivateField(ui, "hitButton", hitButton);
        SetPrivateField(ui, "standButton", standButton);
        SetPrivateField(ui, "exitButton", exitButton);
        SetPrivateField(ui, "backCardSprite", backCardSprite);
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

        var resultText = GetPrivateField<TextMeshProUGUI>(_ui, "resultText");
        var playerCardArea = GetPrivateField<Transform>(_ui, "playerCardArea");
        var dealerCardArea = GetPrivateField<Transform>(_ui, "dealerCardArea");

        Assert.AreEqual(" ", resultText.text);
        Assert.AreEqual(1, playerCardArea.childCount);
        Assert.AreEqual(1, dealerCardArea.childCount);
    }

    [UnityTest]
    public IEnumerator Hit_WhenGameIsActive_ShouldDealCardToPlayer()
    {
        _blackjackGame.StartBlackjack();
        yield return new WaitForSeconds(1.5f);

        var playerCardArea = GetPrivateField<Transform>(_ui, "playerCardArea");
        int initialCardCount = playerCardArea.childCount;

        _blackjackGame.Hit();
        yield return new WaitForSeconds(0.5f);

        Assert.Greater(playerCardArea.childCount, initialCardCount);
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }

    private T GetPrivateField<T>(object obj, string fieldName) where T : class
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        return field != null ? field.GetValue(obj) as T : null;
    }
}
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class BlackjackHandTests
{
    private BlackjackHand _hand;
    private Sprite _mockSprite;

    [SetUp]
    public void SetUp()
    {
        _hand = new BlackjackHand();
        _mockSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
    }

    [Test]
    public void Clear_ShouldRemoveAllCardsFromHand()
    {
        _hand.AddCard(new Card("Card_10", 10, _mockSprite));

        _hand.Clear();

        Assert.AreEqual(0, _hand.CardCount);
    }

    [Test]
    public void GetScore_WithNormalCards_ShouldReturnSumOfValues()
    {
        _hand.AddCard(new Card("Card_10", 10, _mockSprite));
        _hand.AddCard(new Card("Card_5", 5, _mockSprite));

        int score = _hand.GetScore();

        Assert.AreEqual(15, score);
    }

    [Test]
    public void GetScore_WithAceAndNotBusting_ShouldCountAceAsEleven()
    {
        _hand.AddCard(new Card("Card_A", 11, _mockSprite));
        _hand.AddCard(new Card("Card_9", 9, _mockSprite));

        int score = _hand.GetScore();

        Assert.AreEqual(20, score);
    }

    [Test]
    public void GetScore_WithAceAndBusting_ShouldCountAceAsOne()
    {
        _hand.AddCard(new Card("Card_A", 11, _mockSprite));
        _hand.AddCard(new Card("Card_K", 10, _mockSprite));
        _hand.AddCard(new Card("Card_5", 5, _mockSprite));

        int score = _hand.GetScore();

        Assert.AreEqual(16, score);
    }

    [Test]
    public void HasBlackjack_WithTwoCardsSummingTo21_ShouldReturnTrue()
    {
        _hand.AddCard(new Card("Card_A", 11, _mockSprite));
        _hand.AddCard(new Card("Card_10", 10, _mockSprite));

        Assert.IsTrue(_hand.HasBlackjack());
    }

    [Test]
    public void HasBlackjack_WithThreeCardsSummingTo21_ShouldReturnFalse()
    {
        _hand.AddCard(new Card("Card_10", 10, _mockSprite));
        _hand.AddCard(new Card("Card_6", 6, _mockSprite));
        _hand.AddCard(new Card("Card_5", 5, _mockSprite));

        Assert.IsFalse(_hand.HasBlackjack());
    }

    [Test]
    public void GetScoreWithoutFirstCard_ShouldIgnoreFirstCardScore()
    {
        _hand.AddCard(new Card("Card_10", 10, _mockSprite));
        _hand.AddCard(new Card("Card_7", 7, _mockSprite));

        int score = _hand.GetScoreWithoutFirstCard();

        Assert.AreEqual(7, score);
    }
}
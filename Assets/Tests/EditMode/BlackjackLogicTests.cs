using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class BlackjackLogicTests
{
    private List<Sprite> tempSprites;
    private List<Texture2D> tempTextures;

    [SetUp]
    public void Setup()
    {
        tempSprites = new List<Sprite>();
        tempTextures = new List<Texture2D>();
    }

    [TearDown]
    public void Teardown()
    {
        foreach (var s in tempSprites)
            if (s != null) Object.DestroyImmediate(s);

        foreach (var t in tempTextures)
            if (t != null) Object.DestroyImmediate(t);

        tempSprites.Clear();
        tempTextures.Clear();
    }

    private Sprite CreateMockSprite(string spriteName)
    {
        Texture2D tex = new Texture2D(2, 2);
        tempTextures.Add(tex);
        Sprite s = Sprite.Create(tex, new Rect(0, 0, 2, 2), Vector2.zero);
        s.name = spriteName;
        tempSprites.Add(s);
        return s;
    }

    [Test]
    public void CardConstructorSetsValuesProperly()
    {
        Sprite s = CreateMockSprite("test");
        Card card = new Card("hearts_10", 10, s);

        Assert.AreEqual("hearts_10", card.cardName);
        Assert.AreEqual(10, card.value);
        Assert.AreEqual(s, card.sprite);
    }

    [Test]
    public void HandCalculatesScoreCorrectlyWithoutAces()
    {
        BlackjackHand hand = new BlackjackHand();
        hand.AddCard(new Card("hearts_10", 10, null));
        hand.AddCard(new Card("spades_7", 7, null));

        Assert.AreEqual(17, hand.GetScore());
    }

    [Test]
    public void HandCalculatesScoreCorrectlyWithAces()
    {
        BlackjackHand hand = new BlackjackHand();
        hand.AddCard(new Card("hearts_A", 11, null));
        hand.AddCard(new Card("spades_A", 11, null));
        hand.AddCard(new Card("clubs_9", 9, null));

        Assert.AreEqual(21, hand.GetScore());
    }

    [Test]
    public void HandHasBlackjackReturnsTrueOnlyForTwoCardsTotalingTwentyOne()
    {
        BlackjackHand blackjackHand = new BlackjackHand();
        blackjackHand.AddCard(new Card("hearts_A", 11, null));
        blackjackHand.AddCard(new Card("spades_K", 10, null));

        BlackjackHand threeCardHand = new BlackjackHand();
        threeCardHand.AddCard(new Card("hearts_7", 7, null));
        threeCardHand.AddCard(new Card("spades_4", 4, null));
        threeCardHand.AddCard(new Card("clubs_K", 10, null));

        Assert.IsTrue(blackjackHand.HasBlackjack());
        Assert.IsFalse(threeCardHand.HasBlackjack());
    }

    [Test]
    public void HandGetScoreWithoutFirstCardIgnoresFirstCard()
    {
        BlackjackHand hand = new BlackjackHand();
        hand.AddCard(new Card("hearts_A", 11, null));
        hand.AddCard(new Card("spades_5", 5, null));
        hand.AddCard(new Card("clubs_4", 4, null));

        Assert.AreEqual(9, hand.GetScoreWithoutFirstCard());
    }

    [Test]
    public void HandClearRemovesAllCards()
    {
        BlackjackHand hand = new BlackjackHand();
        hand.AddCard(new Card("hearts_10", 10, null));
        
        hand.Clear();

        Assert.AreEqual(0, hand.CardCount);
        Assert.AreEqual(0, hand.GetScore());
    }

    [Test]
    public void DeckSetupIgnoresInvalidCardsAndParsesValuesCorrectly()
    {
        BlackjackDeck deck = new BlackjackDeck();
        Sprite[] sprites = new Sprite[]
        {
            null,
            CreateMockSprite("cardBackRed"),
            CreateMockSprite("hearts_A"),
            CreateMockSprite("spades_K"),
            CreateMockSprite("clubs_7"),
            CreateMockSprite("invalid_string")
        };

        deck.Setup(sprites);

        FieldInfo deckField = typeof(BlackjackDeck).GetField("deck", BindingFlags.NonPublic | BindingFlags.Instance);
        List<Card> internalDeck = (List<Card>)deckField.GetValue(deck);

        Assert.AreEqual(4, internalDeck.Count);

        int totalExpectedValue = 11 + 10 + 7 + 0;
        int actualValue = 0;
        foreach (var card in internalDeck)
        {
            actualValue += card.value;
        }

        Assert.AreEqual(totalExpectedValue, actualValue);
    }

    [Test]
    public void DeckDrawReturnsCardAndReducesCount()
    {
        BlackjackDeck deck = new BlackjackDeck();
        Sprite[] sprites = new Sprite[] { CreateMockSprite("hearts_10") };
        deck.Setup(sprites);

        Card drawn = deck.Draw();
        Card drawnEmpty = deck.Draw();

        Assert.IsNotNull(drawn);
        Assert.AreEqual(10, drawn.value);
        Assert.IsNull(drawnEmpty);
    }
}
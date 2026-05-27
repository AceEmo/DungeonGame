using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class BlackjackDeckTests
{
    private BlackjackDeck _deck;
    private Sprite[] _mockSprites;

    [SetUp]
    public void SetUp()
    {
        _deck = new BlackjackDeck();

        _mockSprites = new Sprite[4];
        _mockSprites[0] = CreateMockSprite("club_10");
        _mockSprites[1] = CreateMockSprite("diamond_A");
        _mockSprites[2] = CreateMockSprite("heart_K");
        _mockSprites[3] = CreateMockSprite("cardBackRed");
    }

    private Sprite CreateMockSprite(string name)
    {
        Sprite sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        sprite.name = name;
        return sprite;
    }

    [Test]
    public void Setup_ShouldPopulateDeckAndIgnoreCardBack()
    {
        _deck.Setup(_mockSprites);

        int count = 0;
        while (_deck.Draw() != null)
        {
            count++;
        }

        Assert.AreEqual(3, count);
    }

    [Test]
    public void Draw_FromEmptyDeck_ShouldReturnNull()
    {
        Card card = _deck.Draw();

        Assert.IsNull(card);
    }

    [Test]
    public void Setup_ShouldCorrectlyParseCardValues()
    {
        _deck.Setup(_mockSprites);

        for (int i = 0; i < 3; i++)
        {
            Card card = _deck.Draw();
            if (card.cardName.EndsWith("_10")) Assert.AreEqual(10, card.value);
            if (card.cardName.EndsWith("_A")) Assert.AreEqual(11, card.value);
            if (card.cardName.EndsWith("_K")) Assert.AreEqual(10, card.value);
        }
    }
}
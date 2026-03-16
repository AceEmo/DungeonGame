using System.Collections.Generic;

public class BlackjackHand
{
    private List<Card> cards = new List<Card>();

    public void Clear()
    {
        cards.Clear();
    }

    public bool HasBlackjack()
    {
        return cards.Count == 2 && GetScore() == 21;
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public int GetScore()
    {
        return ComputeScore(0);
    }

    public int GetScoreWithoutFirstCard()
    {
        if (cards.Count <= 1)
            return 0;

        return ComputeScore(1);
    }

    private int ComputeScore(int startIndex)
    {
        int total = 0;
        int aces = 0;

        for (int i = startIndex; i < cards.Count; i++)
        {
            Card c = cards[i];
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

    public int CardCount => cards.Count;
    public List<Card> Cards => cards;
}

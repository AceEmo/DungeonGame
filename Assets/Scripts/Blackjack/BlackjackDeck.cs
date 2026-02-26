using System.Collections.Generic;
using UnityEngine;

public class BlackjackDeck
{
    private List<Card> deck = new List<Card>();

    public void Setup(Sprite[] cardSprites)
    {
        deck.Clear();

        foreach (Sprite s in cardSprites)
        {
            if (s == null) continue;
            if (s.name == "cardBackRed") continue;

            int value = GetCardValueFromName(s.name);
            deck.Add(new Card(s.name, value, s));
        }
    }

    public Card Draw()
    {
        if (deck.Count == 0) return null;

        int index = Random.Range(0, deck.Count);
        Card card = deck[index];
        deck.RemoveAt(index);
        return card;
    }

    private int GetCardValueFromName(string name)
    {
        int underscoreIndex = name.LastIndexOf('_');
        string rank = name.Substring(underscoreIndex + 1);

        if (rank == "A") return 11;
        if (rank == "K" || rank == "Q" || rank == "J") return 10;

        int parsed;
        if (!int.TryParse(rank, out parsed))
            return 0;

        return parsed;
    }
}
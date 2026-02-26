using UnityEngine;

[System.Serializable]
public class Card
{
    public string cardName;
    public int value;
    public Sprite sprite;

    public Card(string name, int value, Sprite sprite)
    {
        this.cardName = name;
        this.value = value;
        this.sprite = sprite;
    }
}
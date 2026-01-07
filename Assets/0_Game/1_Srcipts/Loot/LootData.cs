using UnityEngine;

public enum LootType
{
    Gold,
    Card,
    Relic
}

[System.Serializable]
public class LootData
{
    public LootType lootType;
    public Sprite icon;
    public string name;
    public int amount; 

    public CardData card;
    public Relic relic;
}

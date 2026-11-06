using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Mellee,
    Ranged,
    Defense,
    Magic,
    Special,
}

public enum EffectOn
{
    Self,
    Opponent,
    All,
}

[System.Serializable]
public class EffectWrapper
{
    public EffectData effect;
    public bool overrideValue;
    public int valueOverride;
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;
    public CardType cardType;
    public EffectOn effectOn;

    [Header("Visuals")]
    public Sprite artwork; 

    [Header("Cost & Description")]
    public int manaCost;
    [TextArea] public string description;

    [Header("Effects")]
    public EffectWrapper[] effects;

    [Header("Shop")]
    public int goldCost = 10;

    [Header("Keyword Info")]
    public bool hasKeywordInfo;
    public List<ConditionType> GetKeywords() => DescriptionFormatter.GetKeywords(description);

   // public bool IsSpecial() => cardType == CardType.Special;
    public bool IsSelfCast() => effectOn == EffectOn.Self;
    public bool IsOpponentCast() => effectOn == EffectOn.Opponent;
    public bool IsAllCast() => effectOn == EffectOn.All;
}

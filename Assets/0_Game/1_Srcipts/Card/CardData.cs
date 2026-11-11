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

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName;
    public CardType cardType;
    public EffectOn effectOn;

    [Header("Card Settings")]
    public bool isUnplayable = false;

    [Header("Visuals")]
    public Sprite artwork;
    public Vector2 artworkSize = new Vector2(150f, 150f);
    public Vector2 artworkPosition = Vector2.zero;
    public float artworkRotation = 0f;

    [Header("Attack Impact")]
    public ImpactProfile impactProfile;

    [Header("ManaCost & Description")]
    public int manaCost;
    [TextArea] public string description;
    [TextArea] public string upgradedDescription;

    [Header("Upgrade")]
    public bool canUpgrade = true;
    public int maxLevel = 2;
    public int upgradeCost = 50;

    [Header("Requirements")]
    public RequirementData[] requirements;

    [Header("Effects")]
    public EffectWrapper[] effects;

    [Header("Shop")]
    public int goldCost = 10;

    [Header("Keyword Info")]
    [HideInInspector] public bool hasKeywordInfo;
    public List<ConditionType> GetKeywords(int level = 1) =>
        DescriptionFormatter.GetKeywords(GetDescription(level));

    //Helper
    public bool CheckRequirements(RequirementContext ctx, out string failureMessage)
    {
        failureMessage = "";
        if (requirements == null) return true;
        foreach (var requirement in requirements)
        {
            if (requirement == null) continue;
            if (!requirement.IsMet(ctx))
            {
                failureMessage = requirement.GetFailureMessage();
                return false;
            }
        }
        return true;
    }
    public string GetDescription(int level) => (level >= 2 && !string.IsNullOrEmpty(upgradedDescription)) ? upgradedDescription : description;
    public string GetName(int level) => level >= 2 ? cardName + "+" : cardName;
    // public bool IsSpecial() => cardType == CardType.Special;
    public bool IsSelfCast() => effectOn == EffectOn.Self;
    public bool IsOpponentCast() => effectOn == EffectOn.Opponent;
    public bool IsAllCast() => effectOn == EffectOn.All;
}

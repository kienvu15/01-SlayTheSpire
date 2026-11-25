using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardInstance
{
    public CardData data;
    public int permanentLevel = 1;                 // lưu theo run

    [System.NonSerialized] public int tempBonus;   // chỉ trong trận, không lưu

    // Level hiệu lực: mọi chỗ cũ dùng card.level vẫn đúng
    public int level => Mathf.Min(permanentLevel + tempBonus, data.maxLevel);

    public CardInstance(CardData data, int level = 1)
    {
        this.data = data;
        this.permanentLevel = level;
    }

    public bool IsUpgraded => level >= 2;
    public bool IsTempUpgraded => tempBonus > 0 && level > permanentLevel;

    // Nâng cấp tạm (trong trận)
    public bool CanUpgrade => data.canUpgrade && level < data.maxLevel;
    // Nâng cấp vĩnh viễn (shop/rest site)
    public bool CanUpgradePermanent => data.canUpgrade && permanentLevel < data.maxLevel;

    public int ManaCost => data.manaCost;
    public string Name => data.GetName(level);
    public string Description => data.GetDescription(level);
    public Sprite Artwork => data.artwork;

    public bool UpgradePermanent()
    {
        if (!CanUpgradePermanent) return false;
        permanentLevel++;
        return true;
    }

    public bool UpgradeTemp()
    {
        if (!CanUpgrade) return false;
        tempBonus++;
        return true;
    }

    public void ClearTempUpgrade() => tempBonus = 0;
}
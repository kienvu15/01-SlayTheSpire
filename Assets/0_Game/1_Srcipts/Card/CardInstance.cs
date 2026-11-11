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

public static class CardEffectRunner
{
    public static bool Run(CardInstance card, Character self, Character target,
                           ManaSystem mana, Deck deck,
                           Character player = null, List<EnemyView> enemies = null,
                           Discard discard = null,
                           CardPileViewer cardPileViewer = null)
    { 
        if (card.data.effects == null) return true;

        var ctx = new EffectContext
        {
            self = self,
            target = target,
            manaSystem = mana,
            deck = deck,
            discard = discard,
            cardPileViewer = cardPileViewer,
            level = card.level,
            sourceCard = card,
            impactProfile = card.data.impactProfile,
            player = player,
            enemies = enemies
        };

        Character dragTarget = target;
        bool allSuccess = true;

        foreach (var wrapper in card.data.effects)
        {
            if (wrapper == null || wrapper.effect == null) continue;

            wrapper.FillContext(ctx);
            ctx.target = dragTarget;

            // Effect không cần target: chạy đúng 1 lần, bất kể enemy còn sống hay không
            if (!wrapper.effect.RequiresTarget)
            {
                if (wrapper.ShouldRun(ctx) && !wrapper.effect.Apply(ctx))
                    allSuccess = false;
                continue;
            }

            foreach (var t in wrapper.ResolveTargets(ctx).ToArray())
            {
                if (t != null && t != ctx.self && t.stats.currentHP <= 0) continue;

                ctx.target = t;
                if (!wrapper.ShouldRun(ctx)) continue;
                if (!wrapper.effect.Apply(ctx)) allSuccess = false;
            }
        }
        return allSuccess;
    }
}
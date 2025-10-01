using UnityEngine;

[CreateAssetMenu(fileName = "AttackEffect_Fragile", menuName = "Cards/Effects/Attack_Fragile")]
public class AttackEffect_Fragile : EffectData, IOverrideValue
{
    public int damage = 4;
    public CardType vfxType = CardType.Mellee;

    public int fragileDuration = 1;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null)
        {
            Debug.LogWarning("[AttackEffect_Fragile] Target = null!");
            return false;
        }

        bool hit = self.DealDamage(target, damage, vfxType);

        if (hit)
        {
            // Apply Fragile condition
            Condition fragile = new Condition
            {
                type = ConditionType.Fragile,
                duration = fragileDuration,
                level = 1
            };

            bool isFromPlayer = self is Player;
            target.AddCondition(fragile, isFromPlayer, CardType.Special);
            Debug.Log($"[AttackEffect_Fragile] Applied Fragile x{fragile.level} to {target.name}");
        }

        return hit;
    }

    // ========== Override Value ==========
    public override int GetIntentValue()
    {
        return damage;
    }

    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? damage;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;

        int old = damage;
        damage = overrideValue;
        bool hit = self.DealDamage(target, damage, vfxType);

        if (hit)
        {
            Condition fragile = new Condition
            {
                type = ConditionType.Fragile,
                duration = fragileDuration,
                level = 1
            };

            bool isFromPlayer = self is Player;
            target.AddCondition(fragile, isFromPlayer, CardType.Special);
            Debug.Log($"[AttackEffect_Fragile] Applied Fragile x{fragile.level} to {target.name} (override)");
        }

        damage = old;
    }
}

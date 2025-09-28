using UnityEngine;

[CreateAssetMenu(fileName = "AttackEffect_Weakened", menuName = "Cards/Effects/Attack_Weakened")]
public class AttackEffect_Weakened : EffectData, IOverrideValue
{
    public int damage = 6;
    public CardType vfxType = CardType.Mellee;

    public int weakenedDuration = 1;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null)
        {
            Debug.LogWarning("[AttackEffect_Weakened] Target = null!");
            return false;
        }

        bool hit = self.DealDamage(target, damage, vfxType);

        if (hit)
        {
            // Apply Weakened condition x1
            Condition weakened = new Condition
            {
                type = ConditionType.Weakened,
                duration = weakenedDuration,
                level = 1
            };

            bool isFromPlayer = self is Player;
            target.AddCondition(weakened, isFromPlayer,CardType.Special);
            Debug.Log($"[AttackEffect_Weakened] Applied Weakened x{weakened.level} to {target.name}");
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
            Condition weakened = new Condition
            {
                type = ConditionType.Weakened,
                duration = weakenedDuration,
                level = 1
            };

            bool isFromPlayer = self is Player;
            target.AddCondition(weakened, isFromPlayer, CardType.Special);
            Debug.Log($"[AttackEffect_Weakened] Applied Weakened x{weakened.level} to {target.name} (override)");
        }

        damage = old;
    }
}

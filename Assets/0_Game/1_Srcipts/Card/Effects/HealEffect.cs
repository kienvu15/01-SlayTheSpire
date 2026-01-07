using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Cards/Effects/Heal")]
public class HealEffect : EffectData, IOverrideValue
{
    public int healAmount = 7;
    public CardType vfxType = CardType.Special;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        Character realTarget = target != null ? target : self;

        if (realTarget == null)
        {
            Debug.LogWarning("[HealEffect] No valid target to apply heal!");
            return false; // ❌ fail
        }

        realTarget.Heal(healAmount);
        Debug.Log($"[HealEffect] {target.name} healed {healAmount} HP");
        return true;
    }

    // ========== Override Value ==========
    public override int GetIntentValue()
    {
        return healAmount;
    }

    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? healAmount;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) target = self;

        int old = healAmount;
        healAmount = overrideValue;

        target.Heal(healAmount);
        Debug.Log($"[HealEffect] {target.name} healed {healAmount} HP (override)");

        healAmount = old;
    }
}

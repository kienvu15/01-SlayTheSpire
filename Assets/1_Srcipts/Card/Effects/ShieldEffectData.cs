using UnityEngine;

[CreateAssetMenu(fileName = "ShiledEffect", menuName = "Cards/Effects/Shield")]
public class ShieldEffectData : EffectData, IOverrideValue
{
    public int baseShield = 5;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        Character realTarget = target != null ? target : self;

        if (realTarget == null)
        {
            Debug.LogWarning("[ShieldEffect] No valid target to apply shield!");
            return false; // ❌ fail
        }

        realTarget.AddShield(baseShield);
        Debug.Log($"[ShieldEffect] {self.name} added {baseShield} shield to {realTarget.name}");
        return true; // ✅ success
    }

    public override int GetIntentValue()
    {
        return baseShield;
    }

    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? baseShield;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        Character realTarget = target != null ? target : self;
        if (realTarget == null) return;

        int old = baseShield;
        baseShield = overrideValue;
        Apply(self, realTarget, null, null);
        baseShield = old;
    }
}
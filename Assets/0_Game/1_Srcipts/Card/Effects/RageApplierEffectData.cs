using UnityEngine;

[CreateAssetMenu(fileName = "RageEffect", menuName = "Cards/Effects/Apply_Rage")]
public class RageApplierEffectData : EffectData, IOverrideValue
{
    [Tooltip("Số stack Rage được cộng")]
    public int rageAmount = 2;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        Character realTarget = target != null ? target : self;
        if (realTarget == null) return false;

        ApplyRage(self, realTarget, rageAmount);
        return true;
    }

    private void ApplyRage(Character self, Character target, int amount)
    {
        Condition rage = new Condition
        {
            type = ConditionType.Rage,
            level = amount,
            duration = 0 // Rage dạng stack, không cần duration
        };

        bool isFromPlayer = self is Player;
        target.AddCondition(rage, isFromPlayer, CardType.Special);

        //Debug.Log($"[RageEffect] {self.name} applied Rage +{amount} to {target.name}");
    }

    // ========== Intent ==========
    public override int GetIntentValue()
    {
        return rageAmount;
    }

    // ========== IOverrideValue ==========
    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? rageAmount;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        Character realTarget = target != null ? target : self;
        if (realTarget == null) return;

        ApplyRage(self, realTarget, overrideValue);
    }
}

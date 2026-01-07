using UnityEngine;

[CreateAssetMenu(fileName = "InjuryEffect", menuName = "Cards/Effects/Injury")]
public class InjuryEffectData : EffectData, IOverrideValue
{
    public int level = 1;      
    public int duration = 1;  

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return false;

        Condition injury = new Condition
        {
            type = ConditionType.Injury,
            duration = duration,
            level = level
        };

        bool isFromPlayer = self is Player;
        target.AddCondition(injury, isFromPlayer);

        Debug.Log($"[InjuryEffect] {self.name} applied Injury({level}) for {duration} turns to {target.name}");
        return true;
    }

    public override int GetIntentValue()
    {
        return level; 
    }

    // --- IOverrideValue ---
    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? level;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;

        Condition injury = new Condition
        {
            type = ConditionType.Injury,
            duration = duration,
            level = overrideValue
        };

        bool isFromPlayer = self is Player;
        target.AddCondition(injury, isFromPlayer);

        Debug.Log($"[InjuryEffect] {self.name} applied Injury({overrideValue}) for {duration} turns to {target.name}");
    }
}

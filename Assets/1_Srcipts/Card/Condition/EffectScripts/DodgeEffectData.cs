using UnityEngine;

[CreateAssetMenu(fileName = "DodgeEffect", menuName = "Cards/Effects/Dodge")]
public class DodgeEffectData : EffectData, IOverrideValue
{
    public int duration = 1;  // số lượt tồn tại mặc định

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return false;

        Condition dodge = new Condition
        {
            type = ConditionType.Dodge,
            duration = duration
        };

        bool isFromPlayer = self is Player;
        target.AddCondition(dodge, isFromPlayer);

        Debug.Log($"[DodgeEffect] {self.name} applied Dodge({duration}) to {target.name}");
        return true;
    }

    public override int GetIntentValue()
    {
        return duration; // hiển thị số lượt Dodge
    }

    // --- IOverrideValue ---
    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? duration;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;

        Condition dodge = new Condition
        {
            type = ConditionType.Dodge,
            duration = overrideValue
        };

        bool isFromPlayer = self is Player;
        target.AddCondition(dodge, isFromPlayer);

        Debug.Log($"[DodgeEffect] {self.name} applied Dodge({overrideValue}) to {target.name}");
    }
}

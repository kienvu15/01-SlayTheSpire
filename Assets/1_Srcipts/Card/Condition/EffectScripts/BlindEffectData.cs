using UnityEngine;

[CreateAssetMenu(fileName = "BlindEffect", menuName = "Cards/Effects/Blind")]
public class BlindEffectData : EffectData, IOverrideValue
{
    [Tooltip("Số lượt tồn tại Blind")]
    public int duration = 1;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return false;

        Condition blind = new Condition
        {
            type = ConditionType.Blind,
            duration = duration
        };

        target.AddCondition(blind);

        Debug.Log($"[BlindEffect] {self.name} applied Blind({duration}) to {target.name}");
        return true;
    }

    public override int GetIntentValue()
    {
        return duration; // hiển thị số lượt Blind
    }

    // --- IOverrideValue ---
    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? duration;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;

        Condition blind = new Condition
        {
            type = ConditionType.Blind,
            duration = overrideValue
        };

        target.AddCondition(blind);

        Debug.Log($"[BlindEffect] {self.name} applied Blind({overrideValue}) to {target.name}");
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "ApplyWeakenedEffect", menuName = "Cards/Effects/Apply_Weakened")]
public class ApplyWeakenedEffectData : EffectData, IOverrideValue
{
    [Tooltip("Số lượt tồn tại Weakened")]
    public int weakenedDuration = 2;


    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return false;

        ApplyWeakened(self, target, weakenedDuration);
        return true;
    }

    private void ApplyWeakened(Character self, Character target, int duration)
    {
        Condition weakened = new Condition
        {
            type = ConditionType.Weakened,
            duration = duration,
            level = 1
        };

        bool isFromPlayer = self is Player;
        target.AddCondition(weakened, isFromPlayer, CardType.Special);

        //Debug.Log($"[ApplyWeakened] {self.name} applied Weakened({duration}) to {target.name}");
    }

    // ========== Intent ==========
    public override int GetIntentValue()
    {
        return weakenedDuration;
    }

    // ========== IOverrideValue ==========
    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? weakenedDuration;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;

        ApplyWeakened(self, target, overrideValue);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "AttackInjuryEffect", menuName = "Cards/Effects/Attack + Injury")]
public class AttackInjuryEffect : EffectData, IOverrideValue
{
    public int damage = 5;
    public int injuryDuration = 2;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return false;

        bool hit = self.DealDamage(target, damage);
        if (hit)
        {
            Debug.Log($"[AttackInjuryEffect] {self.name} dealt {damage} dmg to {target.name}");

            // ✅ add Injury
            Condition injury = new Condition
            {
                type = ConditionType.Injury,
                duration = injuryDuration,
            };
            target.AddCondition(injury);

            Debug.Log($"[AttackInjuryEffect] {target.name} gained Injury for {injuryDuration} turns");
        }
        else
        {
            Debug.Log($"[AttackInjuryEffect] {self.name} missed {target.name}!");
        }

        return true;
    }

    public override int GetIntentValue()
    {
        return damage;
    }

    // --- IOverrideValue ---
    public int GetIntentValue(int? overrideValue = null)
    {
        return (overrideValue ?? damage) + injuryDuration;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;

        int oldDamage = damage;
        damage = overrideValue;

        bool hit = self.DealDamage(target, damage);
        if (hit)
        {
            Debug.Log($"[AttackInjuryEffect] {self.name} dealt {damage} dmg (override) to {target.name}");

            Condition injury = new Condition
            {
                type = ConditionType.Injury,
                duration = injuryDuration,
            };
            target.AddCondition(injury);
        }
        else
        {
            Debug.Log($"[AttackInjuryEffect] {self.name} missed {target.name} (override)!");
        }

        damage = oldDamage;
    }
}

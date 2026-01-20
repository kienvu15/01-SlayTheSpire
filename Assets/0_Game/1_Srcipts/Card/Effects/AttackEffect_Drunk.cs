using UnityEngine;

[CreateAssetMenu(fileName = "AttackEffect_Drunk", menuName = "Cards/Effects/Attack_Drunk")]
public class AttackEffect_Drunk : EffectData, IOverrideValue
{
    public int damage = 6;
    public CardType vfxType = CardType.Mellee;

    [Tooltip("Số lượt tồn tại Drunk")]
    public int drunkDuration = 1;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null)
            return false;

        bool hit = self.DealDamage(target, damage, vfxType);

        if (hit)
        {
            Condition drunk = new Condition
            {
                type = ConditionType.Drunk,
                duration = drunkDuration,
                level = 1
            };

            bool isFromPlayer = self is Player;
            target.AddCondition(drunk, isFromPlayer, CardType.Special);

            //Debug.Log($"[AttackEffect_Drunk] Applied Drunk({drunkDuration}) to {target.name}");
        }

        return hit;
    }

    // ========== Intent ==========
    public override int GetIntentValue()
    {
        return damage;
    }

    // ========== IOverrideValue ==========
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
            Condition drunk = new Condition
            {
                type = ConditionType.Drunk,
                duration = drunkDuration,
                level = 1
            };

            bool isFromPlayer = self is Player;
            target.AddCondition(drunk, isFromPlayer, CardType.Special);
        }

        damage = old;
    }
}

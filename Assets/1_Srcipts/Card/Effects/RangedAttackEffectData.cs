using UnityEngine;

[CreateAssetMenu(fileName = "RangeAttackEffect", menuName = "Cards/Effects/RangeAttack")]
public class RangeAttackEffect : EffectData, IOverrideValue
{
    public int damage = 5;
    public CardType vfxType = CardType.Ranged; // set trong inspector

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null)
        {
            Debug.LogWarning("[RangeAttackEffect] Target = null, attack failed!");
            return false;
        }

        EnemyView enemyTarget = target as EnemyView;
        if (enemyTarget == null || enemyTarget.currentSlot == null) return false;

        bool hit = self.DealDamage(target, damage, vfxType);
        if (hit)
            Debug.Log($"[RangeAttackEffect] {self.name} dealt {damage} damage to {target.name}");
        else
            Debug.Log($"[RangeAttackEffect] {self.name} missed {target.name}!");

        return true;
    }

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
            Debug.Log($"[RangeAttackEffect] {self.name} dealt {overrideValue} damage to {target.name} (override)");
        else
            Debug.Log($"[RangeAttackEffect] {self.name} missed {target.name} (override)");

        damage = old;
    }
}
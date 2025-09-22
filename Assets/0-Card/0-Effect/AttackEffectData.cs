using UnityEngine;

[CreateAssetMenu(fileName = "AttackEffect", menuName = "Cards/Effects/Attack")]
public class AttackEffectData : EffectData, IOverrideValue
{
    public int damage = 5;
    public CardType vfxType = CardType.Mellee; // set trong inspector

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null)
        {
            Debug.LogWarning("[AttackEffect] Target = null, attack failed!");
            return false;
        }

        bool hit = self.DealDamage(target, damage, vfxType);
        if (hit)
            Debug.Log($"[AttackEffect] {self.name} dealt {damage} damage to {target.name}");
        else
            Debug.Log($"[AttackEffect] {self.name} missed {target.name}!");

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
        bool hit = self.DealDamage(target, damage);

        if (hit)
            Debug.Log($"[AttackEffect] {self.name} dealt {overrideValue} damage to {target.name} (override)");
        else
            Debug.Log($"[AttackEffect] {self.name} missed {target.name} (override)");

        damage = old;
    }
}
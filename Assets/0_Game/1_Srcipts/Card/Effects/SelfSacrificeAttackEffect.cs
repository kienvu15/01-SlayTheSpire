using UnityEngine;

[CreateAssetMenu(fileName = "SelfSacrificeAttack", menuName = "Cards/Effects/Self Sacrifice Attack")]
public class SelfSacrificeAttackEffect : EffectData, IOverrideValue
{
    public int damageToTarget = 10;
    public int selfDamage = 2;
    public CardType vfxType = CardType.Mellee;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (self == null)
        {
            Debug.LogWarning("[SelfSacrificeAttack] Self = null!");
            return false;
        }
        if (target == null)
        {
            Debug.LogWarning("[SelfSacrificeAttack] Target = null!");
            return false;
        }

        // Gây damage cho target
        bool hit = self.DealDamage(target, damageToTarget, vfxType);
        if (hit)
            Debug.Log($"[SelfSacrificeAttack] {self.name} dealt {damageToTarget} damage to {target.name}");
        else
            Debug.Log($"[SelfSacrificeAttack] {self.name} missed {target.name}!");

        // Tự trừ máu caster
        self.TakeDamage(selfDamage);
        Debug.Log($"[SelfSacrificeAttack] {self.name} lost {selfDamage} HP due to sacrifice");

        return hit;
    }

    public override int GetIntentValue()
    {
        return damageToTarget;
    }

    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? damageToTarget;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;

        bool hit = self.DealDamage(target, overrideValue, vfxType);

        if (hit)
            Debug.Log($"[SelfSacrificeAttack] {self.name} dealt {overrideValue} damage to {target.name} (override)");
        else
            Debug.Log($"[SelfSacrificeAttack] {self.name} missed {target.name} (override)");

        // tự trừ máu caster giống Apply()
        self.TakeDamage(selfDamage);
        Debug.Log($"[SelfSacrificeAttack] {self.name} lost {selfDamage} HP due to sacrifice");
    }

}

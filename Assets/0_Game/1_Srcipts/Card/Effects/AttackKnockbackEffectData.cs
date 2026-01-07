using UnityEngine;

[CreateAssetMenu(fileName = "AttackKnockbackEffect", menuName = "Cards/Effects/Attack Knockback")]
public class AttackKnockbackEffect : EffectData, IOverrideValue
{
    public int damage = 5;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return false; // ❌ Không có target → trả card về tay

        EnemyView enemy = target as EnemyView;
        if (enemy == null || enemy.currentSlot == null) return false; // ❌ Không phải enemy hợp lệ

        Spot frontSlot = SlotManager.Instance.GetFrontSlot(enemy.currentSlot);
        if (frontSlot != null && frontSlot.isOccupied)
        {
            Debug.LogWarning("[KnockbackEffect] Target không phải enemy đứng đầu!");
            return false; // ❌ card fail → trả về tay
        }

        // ✅ kiểm tra hit
        bool hit = self.DealDamage(target, damage);
        if (!hit)
        {
            Debug.Log($"[KnockbackEffect] {self.name} missed → no knockback.");
            return false; // ❌ miss thì cũng trả card về tay
        }

        // Knockback logic
        Spot current = enemy.currentSlot;
        Spot back = SlotManager.Instance.GetBackSlot(current);

        if (back != null)
        {
            if (!back.isOccupied)
            {
                enemy.StartCoroutine(enemy.Move2Slot(back));
            }
            else
            {
                EnemyView backEnemy = back.occupant;
                enemy.StartCoroutine(enemy.Move2Slot(back));
                backEnemy.StartCoroutine(backEnemy.Move2Slot(current));
            }
        }

        Debug.Log($"[KnockbackEffect] {self.name} dealt {damage} dmg and knocked {enemy.name}.");
        return true; // ✅ thành công → card bị consume
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

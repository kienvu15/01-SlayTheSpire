using UnityEngine;

[CreateAssetMenu(fileName = "AttackKnockbackEffect", menuName = "Cards/Effects/Attack Knockback")]
public class AttackKnockbackEffect : EffectData, IOverrideValue
{
    public int damage = 5;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return true; // vẫn coi là chơi xong

        EnemyView enemy = target as EnemyView;
        if (enemy == null || enemy.currentSlot == null) return true;

        Spot frontSlot = SlotManager.Instance.GetFrontSlot(enemy.currentSlot);
        if (frontSlot != null && frontSlot.isOccupied)
        {
            Debug.LogWarning("[KnockbackEffect] Target không phải enemy đứng đầu!");
            return true; // vẫn dùng bài, nhưng không tác dụng
        }

        // ✅ kiểm tra hit
        bool hit = self.DealDamage(target, damage);
        if (!hit)
        {
            Debug.Log($"[KnockbackEffect] {self.name} missed → no knockback.");
            return true; // vẫn dùng bài thành công, chỉ không có hiệu ứng
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
using UnityEngine;

[CreateAssetMenu(fileName = "PullForwardEffect", menuName = "Cards/Effects/Pull Forward")]
public class PullForwardEffect : EffectData, IOverrideValue
{
    public int damage = 5;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return true;

        EnemyView enemy = target as EnemyView;
        if (enemy == null || enemy.currentSlot == null) return true;

        // ✅ gây damage
        bool hit = self.DealDamage(target, damage);
        if (!hit)
        {
            Debug.Log($"[PullForwardEffect] {self.name} missed {target.name} → no pull.");
            return true;
        }

        // ✅ lấy slot phía trước
        Spot current = enemy.currentSlot;
        Spot front = SlotManager.Instance.GetFrontSlot(current);

        if (front != null)
        {
            if (!front.isOccupied)
            {
                enemy.StartCoroutine(enemy.Move2Slot(front));
            }
            else
            {
                // swap chỗ nếu có enemy đứng trước
                EnemyView frontEnemy = front.occupant;
                enemy.StartCoroutine(enemy.Move2Slot(front));
                frontEnemy.StartCoroutine(frontEnemy.Move2Slot(current));
            }

            Debug.Log($"[PullForwardEffect] {self.name} pulled {enemy.name} forward.");
        }

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
            Debug.Log($"[PullForwardEffect] {self.name} dealt {overrideValue} damage to {target.name} (override)");
        else
            Debug.Log($"[PullForwardEffect] {self.name} missed {target.name} (override)");

        damage = old;
    }
}

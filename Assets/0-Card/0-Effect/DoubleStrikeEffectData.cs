using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DoubleStrikeEffect", menuName = "Cards/Effects/DoubleStrike")]
public class DoubleStrikeEffectData : EffectData
{
    public int frontDamage = 5;
    public float delayBetweenHits = 0.3f; // delay nhỏ cho đẹp
    public CardType vfxType = CardType.Mellee; // set trong inspector


    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return false;

        EnemyView enemyTarget = target as EnemyView;
        if (enemyTarget == null || enemyTarget.currentSlot == null) return false;

        // Check target phải là enemy đứng đầu
        Spot frontSlot = SlotManager.Instance.GetFrontSlot(enemyTarget.currentSlot);
        if (frontSlot != null && frontSlot.isOccupied)
        {
            Debug.LogWarning("[DoubleStrikeEffect] Target không phải enemy đứng đầu!");
            return false; // ❌ fail
        }

        // 👉 Bắt Coroutine từ "self"
        self.StartCoroutine(DoDoubleStrike(self, enemyTarget));
        return true; // ✅ skill cast thành công
    }

    private IEnumerator DoDoubleStrike(Character self, EnemyView enemyTarget)
    {
        // Hit enemy trước
        self.DealDamage(enemyTarget, frontDamage);

        yield return new WaitForSeconds(delayBetweenHits);

        // Hit enemy sau (crit)
        Spot backSlot = SlotManager.Instance.GetBackSlot(enemyTarget.currentSlot);
        if (backSlot != null && backSlot.isOccupied && backSlot.occupant != null)
        {
            float critMultiplier = self.stats.critDamage;
            int backDamage = Mathf.RoundToInt(frontDamage * critMultiplier);

            self.DealDamage(backSlot.occupant, backDamage);
        }
    }

    public override int GetIntentValue()
    {
        
        return frontDamage;
    }
}
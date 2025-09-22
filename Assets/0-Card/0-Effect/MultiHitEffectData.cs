using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiHitEffect", menuName = "Cards/Effects/Multi Hit")]
public class MultiHitEffectData : EffectData, IOverrideValue
{
    public int damage = 3;
    public int hitCount = 3;
    public float delayBetweenHits = 0.2f;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return true;

        // chạy coroutine trên "self"
        self.StartCoroutine(PerformHits(self, target, damage));

        return true; // card effect vẫn coi như thành công
    }

    private IEnumerator PerformHits(Character self, Character target, int baseDamage)
    {
        for (int i = 0; i < hitCount; i++)
        {
            bool hit = self.DealDamage(target, baseDamage); // tự xử lý crit/dodge/multiplier bên trong

            if (hit)
                Debug.Log($"[MultiHitEffect] Hit {i + 1}/{hitCount}: {self.name} dealt {baseDamage} dmg to {target.name}");
            else
                Debug.Log($"[MultiHitEffect] Hit {i + 1}/{hitCount}: {self.name} missed {target.name}");

            if (i < hitCount - 1)
                yield return new WaitForSeconds(delayBetweenHits);
        }
    }



    public override int GetIntentValue()
    {
        return damage;
    }

    public int GetIntentValue(int? overrideValue = null)
    {
        int dmg = overrideValue ?? damage;
        return dmg;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;
        self.StartCoroutine(PerformHitsWithOverride(self, target, overrideValue));
    }

    private IEnumerator PerformHitsWithOverride(Character self, Character target, int overrideValue)
    {
        for (int i = 0; i < hitCount; i++)
        {
            bool hit = self.DealDamage(target, overrideValue);

            if (hit)
                Debug.Log($"[MultiHitEffect] Hit {i + 1}/{hitCount}: {self.name} dealt {overrideValue} dmg (override)");
            else
                Debug.Log($"[MultiHitEffect] Hit {i + 1}/{hitCount}: {self.name} missed (override)");

            if (i < hitCount - 1)
                yield return new WaitForSeconds(delayBetweenHits);
        }
    }
}
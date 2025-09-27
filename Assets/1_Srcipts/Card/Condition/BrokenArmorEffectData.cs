//using UnityEngine;

//[CreateAssetMenu(fileName = "WeakenEffect", menuName = "Cards/Effects/Weaken")]
//public class WeakenEffectData : EffectData, IOverrideValue
//{
//    public int duration = 2;   // số lượt tồn tại mặc định
//    public float damageTakenMultiplier = 1.15f; // nhận thêm 15% sát thương

//    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
//    {
//        if (target == null) return false;

//        Condition weaken = new Condition
//        {
//            type = ConditionType.Weakened,
//            duration = duration,
//            damageTakenMultiplier = damageTakenMultiplier
//        };

//        target.AddCondition(weaken);

//        Debug.Log($"[WeakenEffect] {self.name} applied Weaken({duration}) to {target.name}");
//        return true;
//    }

//    public override int GetIntentValue()
//    {
//        return duration;
//    }

//    // --- IOverrideValue ---
//    public int GetIntentValue(int? overrideValue = null)
//    {
//        return overrideValue ?? duration;
//    }

//    public void ApplyWithOverride(Character self, Character target, int overrideValue)
//    {
//        if (target == null) return;

//        Condition weaken = new Condition
//        {
//            type = ConditionType.Weakened,
//            duration = overrideValue,  // dùng giá trị override
//            damageTakenMultiplier = damageTakenMultiplier
//        };

//        target.AddCondition(weaken);

//        Debug.Log($"[WeakenEffect] {self.name} applied Weaken({overrideValue}) to {target.name}");
//    }
//}

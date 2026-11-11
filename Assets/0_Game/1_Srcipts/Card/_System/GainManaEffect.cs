using UnityEngine;

[CreateAssetMenu(fileName = "GainManaEffect", menuName = "Cards/Effects/Gain Mana")]
public class GainManaEffect : EffectData
{
    public int amount = 1;

    public override bool Apply(EffectContext ctx)
    {
        if (ctx.manaSystem == null) return false;

        ctx.manaSystem.GainMana(ctx.GetValue(amount));
        return true;
    }

    public override int GetIntentValue(EffectContext ctx) => ctx.GetValue(amount);
}
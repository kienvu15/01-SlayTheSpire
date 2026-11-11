using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Shield")]
public class ShieldEffect : EffectData
{
    public int shield = 5;

    public override bool CountsAsIntend => true;

    public override bool Apply(EffectContext ctx)
    {
        Character t = ctx.target != null ? ctx.target : ctx.self;
        if (t == null) return false;
        t.AddShield(ctx.GetValue(shield));
        return true;
    }

    public override int GetIntentValue(EffectContext ctx) => ctx.GetValue(shield);
}
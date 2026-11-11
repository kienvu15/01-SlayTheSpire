using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Apply Condition")]
public class ApplyConditionEffect : EffectData
{
    public ConditionType conditionType;
    public int duration = 1;      // override value = duration
    public int conditionLevel = 1;

    public override bool Apply(EffectContext ctx)
    {
        Character t = ctx.target != null ? ctx.target : ctx.self;
        if (t == null) return false;

        var cond = new Condition
        {
            type = conditionType,
            duration = ctx.GetValue(duration),
            level = conditionLevel
        };

        t.AddCondition(cond, ctx.self is Player, CardType.Special);
        return true;
    }
}
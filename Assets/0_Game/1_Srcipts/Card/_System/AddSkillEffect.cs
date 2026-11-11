using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Add Skill")]
public class AddSkillEffect : EffectData
{
    public SkillType skillType;
    public int stacks = 1;     // override value = số stack
    public int value = 100;    // LifeSteal: % hồi máu

    public override bool Apply(EffectContext ctx)
    {
        // Skill kích hoạt khi CHỦ SỞ HỮU đánh, nên luôn gắn vào self
        if (ctx.self == null) return false;

        ctx.self.AddSkill(new Skill
        {
            type = skillType,
            stacks = Mathf.Max(1, ctx.GetValue(stacks)),
            value = value
        });
        return true;
    }
}
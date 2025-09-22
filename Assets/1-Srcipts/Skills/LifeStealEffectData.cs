using UnityEngine;

[CreateAssetMenu(fileName = "LifeStealEffect", menuName = "Cards/Effects/LifeSteal")]
public class LifeStealEffectData : EffectData
{
    [Range(0f, 1f)] public float lifeStealPercent = 1f; // 1.0 = 100%
    public int stacks = 1; // số lần kích hoạt

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (self == null) return false;

        // tạo skill LifeSteal
        Skill skill = new Skill
        {
            type = SkillType.LifeSteal,
            stacks = stacks
        };

        // gọi Character.AddSkill -> tự cập nhật SkillPanelUI
        self.AddSkill(skill);

        Debug.Log($"[LifeStealEffect] {self.name} gained LifeSteal ({stacks} stack, {lifeStealPercent * 100}% heal).");
        return true;
    }

    public override int GetIntentValue()
    {
        return stacks;
    }
}

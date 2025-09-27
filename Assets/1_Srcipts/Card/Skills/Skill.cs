using UnityEngine;

public enum SkillType
{
    LifeSteal,
    HeadShot,
    Stun,
}

[System.Serializable]
public class Skill
{
    public SkillType type;
    public int stacks = 1;   // số lần có thể kích hoạt

    public Skill Clone() => (Skill)MemberwiseClone();

    /// <summary>
    /// Gọi trước khi damage apply (cho phép chỉnh crit, bonus damage…)
    /// </summary>
    public void OnDealDamage(Character self, Character target, ref int damage, ref bool isCrit)
    {
        switch (type)
        {
            case SkillType.HeadShot:
                // đòn tiếp theo chắc chắn crit
                isCrit = true;
                Debug.Log($"[Skill] {self.name} triggered HeadShot!");
                stacks--;
                break;

            case SkillType.Stun:
                // Stun sẽ xử lý sau khi hit, không làm gì ở đây
                break;

            case SkillType.LifeSteal:
                // Không can thiệp damage, chỉ xử lý ở OnHit
                break;
        }
    }

    /// <summary>
    /// Gọi sau khi damage đã apply (cho heal, stun…)
    /// </summary>
    public void OnHit(Character self, Character target, int damageDealt)
    {
        switch (type)
        {
            case SkillType.LifeSteal:
                int heal = Mathf.RoundToInt(damageDealt * 1f); // 100% heal, tỉ lệ này để EffectData quyết định
                self.Heal(heal);
                Debug.Log($"[Skill] {self.name} healed {heal} HP from LifeSteal!");
                stacks--;
                break;

            case SkillType.Stun:
                if (target != null)
                {
                    target.ApplyStun(1);
                    Debug.Log($"[Skill] {self.name} stunned {target.name}!");
                    stacks--;
                }
                break;
        }
    }
}

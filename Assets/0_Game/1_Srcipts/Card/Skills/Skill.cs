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
    public int stacks = 1;   

    // helper
    public int value = 100;   // LifeSteal: % sát thương hồi lại. Skill khác bỏ qua

    public Skill Clone() => (Skill)MemberwiseClone();

    public bool IsExpired => stacks <= 0;

    // ===== Hook giống Condition: Character gọi để hỏi =====

    /// Đòn kế tiếp có chắc chắn crit không
    public bool ForceCritical() => type == SkillType.HeadShot && stacks > 0;

    /// Gọi sau khi đòn đã trúng. forcedCrit = đòn này crit là do skill ép
    public void OnHit(Character self, Character target, int damageDealt, bool forcedCrit)
    {
        if (stacks <= 0) return;

        switch (type)
        {
            case SkillType.HeadShot:
                if (forcedCrit)
                {
                    stacks--;
                    Debug.Log($"[Skill] {self.name} used HeadShot");
                }
                break;

            case SkillType.LifeSteal:
                int heal = Mathf.RoundToInt(damageDealt * value / 100f);
                if (heal > 0) self.Heal(heal);
                stacks--;
                Debug.Log($"[Skill] {self.name} healed {heal} HP from LifeSteal");
                break;

            case SkillType.Stun:
                if (target != null)
                {
                    target.ApplyStun(1);
                    stacks--;
                    Debug.Log($"[Skill] {self.name} stunned {target.name}");
                }
                break;
        }
    }

}
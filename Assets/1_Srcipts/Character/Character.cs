using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Stats")]
    public CharacterStats stats = new CharacterStats();

    [Header("UI References")]
    public TextMeshProUGUI hpText;
    public Slider hpBar;
    public TextMeshProUGUI shieldText;
    public GameObject Shield;
    
    [Header("Conditions")]
    public int maxConditions = 2;
    public ConditionPanelUI conditionPanelUI;
    private List<Condition> activeConditions = new List<Condition>();

    [Header("Skills")]
    public List<Skill> activeSkills = new List<Skill>();
    public SkillPanelUI skillPanelUI;

    // condi
    public bool forceCritNextAttack = false; // hỗ trợ HeadShot
    public bool isStunned = false;
    public int stunDuration = 0;
    

    [Header("Relics")]
    public RelicManager relicManager;
    protected virtual void Start()
    {
        stats.currentHP = stats.maxHP;
        UpdateUI();
    }

    protected virtual void Update()
    {
        Shield.SetActive(stats.shield > 0);
    }

    // ================== Damage & Heal ==================
    public virtual void TakeDamage(int amount)
    {
        // ===== Check Dodge / Miss từ Conditions =====
        foreach (var cond in activeConditions)
        {
            float dodge = cond.GetDodgeChance();
            if (dodge > 0f && Random.value < dodge)
            {
                Debug.Log($"[TakeDamage] {name} dodged the attack ({cond.type})!");
                return; // hoàn toàn né tránh
            }
        }

        int remaining = amount;

        // ===== Absorb by Shield =====
        if (stats.shield > 0)
        {
            int absorbed = Mathf.Min(stats.shield, remaining);
            stats.shield -= absorbed;
            remaining -= absorbed;
        }

        if (remaining > 0)
        {
            // ===== Multiplier từ Conditions =====
            float multiplier = 1f;
            foreach (var cond in activeConditions)
                multiplier *= cond.GetDamageTakenMultiplier();

            // ===== Armor (Fragile xử lý trong multiplier) =====
            int raw = Mathf.Max(remaining - stats.defense, 0);
            int dmg = Mathf.RoundToInt(raw * multiplier);

            dmg = Mathf.Max(dmg, 0);

            stats.currentHP -= dmg;
            stats.currentHP = Mathf.Max(stats.currentHP, 0);
        }

        UpdateUI();
    }

    public virtual void Heal(int amount)
    {
        stats.currentHP = Mathf.Min(stats.currentHP + amount, stats.maxHP);
        UpdateUI();
    }

    public virtual void AddShield(int amount)
    {
        stats.shield += amount;
        UpdateUI();
    }

    // Deal raw damage, bypass all checks and effects
    public bool DealRawDamage(Character target, int dmg)
    {
        if (target == null) return false;
        target.TakeDamage(dmg);
        return true;
    }

    

    // Deal damage with all checks (miss, dodge, crit, conditions, skills...)
    public bool DealDamage(Character target, int baseDamage, CardType vfxType = CardType.Mellee)
    {
        // ===== Check attacker self-miss (Blind, Drunk, etc) =====
        float selfMissChance = 0f;
        foreach (var cond in activeConditions)
            selfMissChance = Mathf.Max(selfMissChance, cond.GetAttackerMissChance());

        if (selfMissChance > 0f && Random.value < selfMissChance)
        {
            Debug.Log($"[DealDamage] {name} missed due to self condition!");
            DamagePopupManager.Instance.ShowPopup(target.transform.position, 0, true);
            AttackImpactManager.Instance.ShowImpact(vfxType, target.transform);

            return false; // ❌ miss
        }

        // ===== Check target dodge =====
        foreach (var cond in target.activeConditions)
        {
            float dodge = cond.GetDodgeChance();
            if (dodge > 0f && Random.value < dodge)
            {
                Debug.Log($"[DealDamage] {target.name} dodged!");
                DamagePopupManager.Instance.ShowPopup(target.transform.position, 0, true);
                AttackImpactManager.Instance.ShowImpact(vfxType, target.transform);
                return false; // ❌ miss
            }
        }

        // ===== Damage multiplier, crit, etc =====
        float multiplier = 1f;
        foreach (var cond in activeConditions)
            multiplier *= cond.GetDamageDealtMultiplier();

        int dmg = Mathf.RoundToInt(baseDamage * multiplier);

        // 🟢 Relic hook: modify damage trước khi tính crit
        if (relicManager != null)
            dmg = relicManager.ApplyOnBeforeDealDamage(this, target, dmg);


        bool isCrit = false;
        foreach (var cond in activeConditions)
            if (cond.ForceCritical()) { isCrit = true; break; }

        if (!isCrit && Random.value < stats.critChance)
            isCrit = true;

        if (isCrit)
            dmg = Mathf.RoundToInt(dmg * stats.critDamage);

        // ===== Apply damage =====
        target.TakeDamage(dmg);

        // 🟢 Relic hook: sau khi gây damage
        if (relicManager != null)
            relicManager.ApplyOnAfterDealDamage(this, target, dmg);

        // ===== VFX + Popup cho mỗi lần hit =====
        AttackImpactManager.Instance.ShowImpact(vfxType, target.transform);
        DamagePopupManager.Instance.ShowPopup(target.transform.position, dmg, false /*isMiss*/);

        // ===== Post-hit hooks (skills...) =====
        foreach (var skill in activeSkills.ToArray())
        {
            skill.OnHit(this, target, dmg);
            if (skillPanelUI != null)
                skillPanelUI.UpdateStacks(skill.type, skill.stacks);
        }

        foreach (var skill in activeSkills.ToArray())
        {
            if (skill.stacks <= 0)
            {
                if (skillPanelUI != null)
                    skillPanelUI.RemoveSkill(skill.type);
                activeSkills.Remove(skill);
            }
        }

        return true; // ✅ hit
    }

    public void AddCondition(Condition newCondition, bool isFromPlayer, CardType vfxType = CardType.Special)
    {
        // Không thể replace nếu có Immunity
        foreach (var cond in activeConditions)
        {
            if (cond.IsImmune())
            {
                Debug.Log($"[Condition] {name} is immune, cannot add {newCondition.type}");
                return;
            }
        }

        Condition existing = activeConditions.Find(c => c.type == newCondition.type);

        if (existing != null)
        {
            existing.duration += newCondition.duration;
            Debug.Log($"[Condition] {newCondition.type} refreshed, new duration = {existing.duration}");
        }
        else
        {
            if (activeConditions.Count >= maxConditions)
            {
                Condition removed = activeConditions[0];
                removed.OnExpire(this);
                activeConditions.RemoveAt(0);
                Debug.Log($"[Condition] Removed {removed.type}");
            }

            Condition clone = newCondition.Clone();

            // 🟢 Gán owner chi tiết
            if (isFromPlayer)
                clone.owner = (this is Player) ? ConditionOwner.PlayerOnSelf : ConditionOwner.Player;
            else
                clone.owner = (this is EnemyView) ? ConditionOwner.EnemyOnSelf : ConditionOwner.Enemy;

            activeConditions.Add(clone);
            AttackImpactManager.Instance.ShowConditionImpact(transform, vfxType);

            Debug.Log($"[Condition] Added {clone.type} ({clone.duration} turns) from {clone.owner}");
        }

        if (conditionPanelUI != null)
            conditionPanelUI.UpdateConditions(activeConditions);
    }


    // Gọi ở START TURN
    public void TriggerConditionEffects()
    {
        foreach (var cond in activeConditions.ToArray())
        {
            cond.OnTurn(this); // chỉ chạy effect, KHÔNG trừ duration
        }
    }

    // Player tự buff chính mình
    public void DecreasePlayerSelfConditions()
    {
        for (int i = activeConditions.Count - 1; i >= 0; i--)
        {
            var cond = activeConditions[i];
            if (cond.owner == ConditionOwner.PlayerOnSelf)
            {
                cond.duration--;
                if (cond.duration <= 0)
                {
                    cond.OnExpire(this);
                    activeConditions.RemoveAt(i);
                }
            }
        }
        conditionPanelUI?.UpdateConditions(activeConditions);
    }

    // Enemy tự buff chính mình
    public void DecreaseEnemySelfConditions()
    {
        for (int i = activeConditions.Count - 1; i >= 0; i--)
        {
            var cond = activeConditions[i];
            if (cond.owner == ConditionOwner.EnemyOnSelf)
            {
                cond.duration--;
                if (cond.duration <= 0)
                {
                    cond.OnExpire(this);
                    activeConditions.RemoveAt(i);
                }
            }
        }
        conditionPanelUI?.UpdateConditions(activeConditions);
    }


    // Gọi ở END TURN của Player
    public void DecreasePlayerConditions()
    {
        for (int i = activeConditions.Count - 1; i >= 0; i--)
        {
            var cond = activeConditions[i];
            if (cond.owner == ConditionOwner.Player) // chỉ giảm condition do Player cast
            {
                cond.duration--;
                if (cond.duration <= 0)
                {
                    cond.OnExpire(this);
                    activeConditions.RemoveAt(i);
                }
            }
        }
        conditionPanelUI?.UpdateConditions(activeConditions);
    }

    // Gọi ở END TURN của Enemy
    public void DecreaseEnemyConditions()
    {
        for (int i = activeConditions.Count - 1; i >= 0; i--)
        {
            var cond = activeConditions[i];
            if (cond.owner == ConditionOwner.Enemy) // chỉ giảm condition do Enemy cast
            {
                cond.duration--;
                if (cond.duration <= 0)
                {
                    cond.OnExpire(this);
                    activeConditions.RemoveAt(i);
                }
            }
        }
        conditionPanelUI?.UpdateConditions(activeConditions);
    }


    // ================== Skill ==================
    public void AddSkill(Skill newSkill)
    {
        Skill existing = activeSkills.Find(s => s.type == newSkill.type);

        if (existing != null)
        {
            existing.stacks += newSkill.stacks;
            Debug.Log($"[Skill] {newSkill.type} stacked! Total stacks = {existing.stacks}");
        }
        else
        {
            Skill clone = newSkill.Clone();
            activeSkills.Add(clone);
            Debug.Log($"[Skill] Added {newSkill.type} ({newSkill.stacks} stacks)");

            
        }

        if (skillPanelUI != null)
        {
            var currentStacks = activeSkills.Find(s => s.type == newSkill.type).stacks;
            skillPanelUI.UpdateStacks(newSkill.type, currentStacks);
        }
    }

    public void ApplyStun(int turns)
    {
        isStunned = true;
        stunDuration = Mathf.Max(stunDuration, turns);
        Debug.Log($"{name} is stunned for {stunDuration} turns!");
    }

    public void TickStun()
    {
        if (isStunned)
        {
            stunDuration--;
            if (stunDuration <= 0)
            {
                isStunned = false;
                Debug.Log($"{name} recovered from stun!");
            }
        }
    }

    // ================== UI ==================
    public void UpdateUI()
    {
        if (hpText != null) hpText.text = $"{stats.currentHP}/{stats.maxHP}";
        if (shieldText != null) shieldText.text = $"{stats.shield}";
        if (hpBar != null) hpBar.value = (float)stats.currentHP / stats.maxHP;
    }
}

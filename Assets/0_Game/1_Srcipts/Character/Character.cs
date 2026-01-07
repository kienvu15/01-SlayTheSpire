using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum Faction
{
    Player,
    Enemy
}

public class Character : MonoBehaviour
{
    [Header("Basic Info")]
    public Faction faction;
    public bool isAlive = true;

    [Header("Stats")]
    public CharacterStats stats = new CharacterStats();

    [Header("UI References")]
    public TextMeshProUGUI hpText;
    public Slider hpBar;
    public Slider hpChipBar;

    Color shieldColor = new Color32(0x34, 0x7E, 0x91, 0xFF);

    public TextMeshProUGUI shieldText;
    public GameObject Shield;

    [Header("HP Chip Effect")]
    [SerializeField] private float chipDelay = 0.5f;
    [SerializeField] private float chipSpeed = 0.5f;

    private float targetFill;      
    private float chipTimer;       

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

        if (hpChipBar != null)
        {
            hpChipBar.maxValue = 1f;
            hpChipBar.value = 1f;
        }
    }


    protected virtual void Update()
    {
        Shield.SetActive(stats.shield > 0);
        if(stats.shield > 0)
        {
            hpBar.fillRect.GetComponent<Image>().color = shieldColor;
        }
        else
        {
            hpBar.fillRect.GetComponent<Image>().color = Color.red;
        }

        // ==== CHIP EFFECT ====
        if (hpChipBar != null && hpBar != null)
        {
            float currentValue = hpBar.value;
            if (hpChipBar.value > currentValue)
            {
                chipTimer += Time.deltaTime;
                if (chipTimer >= chipDelay)
                {
                    hpChipBar.value = Mathf.MoveTowards(
                        hpChipBar.value,
                        currentValue,
                        Time.deltaTime * chipSpeed
                    );
                }
            }
            else
            {
                // Nếu heal hoặc chip thấp hơn máu thật → theo kịp luôn
                hpChipBar.value = currentValue;
                chipTimer = 0f;
            }
        }
    }

    //======== Clear all conditions and skills (for testing) ========
    public void ClearAllConditionsAndSkills()
    {
        activeConditions.Clear();
        activeSkills.Clear();
        conditionPanelUI?.UpdateConditions(activeConditions);
        skillPanelUI?.ClearAll();
        stats.shield = 0;
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

    public virtual void Heal(int amount, CardType vfxType = CardType.Mellee)
    {
        // Chỉ heal nếu máu chưa đầy hoặc muốn hiển thị effect
        if (amount <= 0) return;

        stats.currentHP = Mathf.Min(stats.currentHP + amount, stats.maxHP);
        UpdateUI();

        // ▶ PLAY VFX HEAL
        // Target là chính mình (this), EffectKind là Heal
        PlayImpact(this, vfxType, ImpactEffectKind.Heal);

        Debug.Log($"[Heal] {name} healed {amount} HP");

        // (Optional) Nếu bạn có popup cho máu, gọi ở đây:
        // DamagePopupManager.Instance.ShowPopup(transform.position, -amount, false); // Ví dụ số âm là màu xanh
    }

    // Cập nhật Method AddShield
    public virtual void AddShield(int amount, CardType vfxType = CardType.Special)
    {
        SoundManager.Instance.Play("ShieldActivate");
        // ===== Multiplier từ Conditions =====
        float multiplier = 1f;
        foreach (var cond in activeConditions)
            multiplier *= cond.GetShieldGainMultiplier();

        int finalShield = Mathf.RoundToInt(amount * multiplier);

        if (finalShield > 0)
        {
            stats.shield += finalShield;
            Debug.Log($"[AddShield] {name} gained {finalShield} shield (base {amount}, multiplier {multiplier})");

            UpdateUI();

            // ▶ PLAY VFX SHIELD
            // Target là chính mình (this), EffectKind là Shield
            PlayImpact(this, vfxType, ImpactEffectKind.Shield);
        }
    }

    #region raw damage
    // Deal raw damage, bypass all checks and effects
    public bool DealRawDamage(Character target, int dmg)
    {
        if (target == null) return false;
        target.TakeDamage(dmg);
        return true;
    }
    #endregion 
    private ImpactEffectKind GetImpactForHit(bool isMiss, bool isDodge, bool isCrit, bool hitArmor)
    {
        if (isMiss) return ImpactEffectKind.Miss;
        if (isDodge) return ImpactEffectKind.Dodge;
        if (isCrit) return ImpactEffectKind.Crit;
        if (hitArmor) return ImpactEffectKind.HitArmor;

        return ImpactEffectKind.Damage;
    }

    // Deal damage with all checks (miss, dodge, crit, conditions, skills...)
    public bool DealDamage(Character target, int baseDamage, CardType vfxType = CardType.Mellee)
    {
        SoundManager.Instance.Play("Slash");  
        // ===== Check attacker miss =====
        float selfMissChance = 0f;
        foreach (var cond in activeConditions)
            selfMissChance = Mathf.Max(selfMissChance, cond.GetAttackerMissChance());

        if (selfMissChance > 0f && Random.value < selfMissChance)
        {
            Debug.Log($"[DealDamage] {name} missed due to self condition!");
            DamagePopupManager.Instance.ShowPopup(target.transform.position, 0, true);

            // Impact VFX cho Miss
            PlayImpact(target, vfxType, ImpactEffectKind.Miss);
            return false;
        }

        // ===== Check target dodge =====
        foreach (var cond in target.activeConditions)
        {
            float dodge = cond.GetDodgeChance();
            if (dodge > 0f && Random.value < dodge)
            {
                Debug.Log($"[DealDamage] {target.name} dodged!");
                DamagePopupManager.Instance.ShowPopup(target.transform.position, 0, true);

                // Impact VFX cho Dodge
                PlayImpact(target, vfxType, ImpactEffectKind.Dodge);
                return false;
            }
        }

        // ===== Damage multiplier =====
        float multiplier = 1f;
        foreach (var cond in activeConditions)
            multiplier *= cond.GetDamageDealtMultiplier();

        int dmg = Mathf.RoundToInt(baseDamage * multiplier);

        // Relic BEFORE
        if (relicManager != null)
            dmg = relicManager.ApplyOnBeforeDealDamage(this, target, dmg);

        // ===== Crit =====
        bool isCrit = false;

        foreach (var cond in activeConditions)
            if (cond.ForceCritical()) { isCrit = true; break; }

        if (!isCrit && Random.value < stats.critChance)
            isCrit = true;

        if (isCrit)
            dmg = Mathf.RoundToInt(dmg * stats.critDamage);

        // ===== Apply damage =====
        int beforeHP = target.stats.currentHP;
        target.TakeDamage(dmg);
        int afterHP = target.stats.currentHP;

        bool isDead = target.stats.currentHP <= 0;

        bool hitArmor = !isDead && ((afterHP - beforeHP) != -dmg);

        // Relic AFTER
        if (relicManager != null)
            relicManager.ApplyOnAfterDealDamage(this, target, dmg);

        ImpactEffectKind finalImpact =
            GetImpactForHit(
                isMiss: false,
                isDodge: false,
                isCrit: isCrit,
                hitArmor: hitArmor 
            );

        PlayImpact(target, vfxType, finalImpact);

        // ===== Damage popup =====
        DamagePopupManager.Instance.ShowPopup(target.transform.position, dmg, isCrit);

        // ===== Skills hook =====
        foreach (var skill in activeSkills.ToArray())
        {
            skill.OnHit(this, target, dmg);
            skillPanelUI?.UpdateStacks(skill.type, skill.stacks);
        }

        // Cleanup skills
        for (int i = activeSkills.Count - 1; i >= 0; i--)
        {
            if (activeSkills[i].stacks <= 0)
            {
                skillPanelUI?.RemoveSkill(activeSkills[i].type);
                activeSkills.RemoveAt(i);
            }
        }

        return true;
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
            PlayImpact(this, vfxType, ImpactEffectKind.Dodge);
            // AttackImpactManager.Instance.ShowConditionImpact(transform, vfxType);

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

        if (hpBar != null)
        {
            float newValue = (float)stats.currentHP / stats.maxHP;
            hpBar.value = newValue;

            // reset timer khi thay đổi máu
            chipTimer = 0f;
            targetFill = newValue;

            // nếu chip chưa có giá trị ban đầu → đồng bộ
            if (hpChipBar != null && hpChipBar.value <= 0)
                hpChipBar.value = newValue;
        }
    }

    // IMPACT CALL
    private void PlayImpact(Character target, CardType cardType, ImpactEffectKind effect)
    {
        HitContext ctx = new HitContext
        {
            cardType = cardType,
            effectKind = effect,
            target = target.transform,
            position = target.transform.position,
            rotation = Quaternion.identity
        };

        NewAttackImpactManager.Instance.PlayImpact(ctx);
    }

    // RANDOM MINI SYSTEM
    private bool RollMiss() => Random.value < 0.05f;
    private bool RollDodge() => Random.value < 0.05f;

    private bool RollCrit(ref int dmg)
    {
        if (Random.value < 0.1f)
        {
            dmg = Mathf.RoundToInt(dmg * 1.5f);
            return true;
        }
        return false;
    }

}

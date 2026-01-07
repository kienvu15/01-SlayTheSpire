using UnityEngine;

public enum ConditionType
{
    Dodge,
    Drunk,
    Immunity,
    Rage,
    Blind,
    Fragile,
    Injury,
    Mark,
    Weakened,
}

public enum ConditionOwner
{
    Player,
    Enemy,
    EnemyOnSelf,
    PlayerOnSelf
}

[System.Serializable]
public class Condition
{
    public ConditionType type;
    public int duration = 1;       // số lượt tồn tại
    public int level = 1;
    public ConditionOwner owner;

    public Condition Clone() => (Condition)MemberwiseClone();

    /// Gọi mỗi lượt → thực hiện hiệu ứng theo thời gian
    public void OnTurn(Character character)
    {
        switch (type)
        {
            case ConditionType.Injury:
                int dmg = Mathf.RoundToInt(duration); // mất HP = level (vd: Injury 2 = 2 HP mỗi lượt)
                character.TakeDamage(dmg);
                Debug.Log($"[Condition] Injury deals {dmg} dmg to {character.name}");
                break;
        }
    }

    /// Gọi khi Condition hết hạn
    public void OnExpire(Character character)
    {
        Debug.Log($"[Condition] {type} expired on {character.name}");
    }

    /// Xác suất né tránh từ Condition
    public float GetDodgeChance()
    {
        switch (type)
        {
            case ConditionType.Dodge:
                return 0.5f;
        }
        return 0f;
    }

    /// Trả xác suất khiến NGƯỜI TẤN CÔNG miss khi tấn công mục tiêu có condition này.
    public float GetAttackerMissChance()
    {
        switch (type)
        {
            case ConditionType.Blind:
                return 0.5f;
            case ConditionType.Drunk:
                return 0.5f; 
        }
        return 0f;
    }

    /// Nhân sát thương gây ra
    public float GetDamageDealtMultiplier()
    {
        switch (type)
        {
            case ConditionType.Rage: return 1.25f;
            case ConditionType.Weakened: return 0.75f;
        }
        return 1f;
    }

    /// Nhân sát thương nhận vào
    public float GetDamageTakenMultiplier()
    {
        switch (type)
        {
            // case ConditionType.Fragile: return 1.5f; // -50% armor effect ~ nhận nhiều dmg hơn
        }
        return 1f;
    }

    /// Nhân giá trị shield được cộng vào
    public float GetShieldGainMultiplier()
    {
        switch (type)
        {
            case ConditionType.Fragile:
                return 0.5f; // chỉ nhận 50% shield
        }
        return 1f;
    }

    /// Check luôn Crit từ condition
    public bool ForceCritical()
    {
        switch (type)
        {
            case ConditionType.Mark:
                return true; // mọi đòn đánh đều crit
            case ConditionType.Drunk:
                return Random.value < 0.5f; // 50% crit
        }
        return false;
    }

    /// Kiểm tra có phải là trạng thái miễn nhiễm (không bị replace)
    public bool IsImmune()
    {
        return type == ConditionType.Immunity;
    }
}

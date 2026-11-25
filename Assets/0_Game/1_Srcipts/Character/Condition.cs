using UnityEngine;

public enum ConditionType
{
    Dodge, // Dodge
    Drunk, // 50% more damage, 50% miss
    Immunity, // cannot gain status
    Rage, // deal more damage
    Blind, // change to miss
    Fragile, // -50% shield gain
    Injury, // deal x damage at turn
    Mark, // force crit
    Weakened,// deal less damage
    Vulnerable, // take more damage
    Poisoned, 
    Entangled
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
    public int duration = 1;       
    public int level = 1;
    public ConditionOwner owner;

    public Condition Clone() => (Condition)MemberwiseClone();

    public void OnTurn(Character character)
    {
        switch (type)
        {
            case ConditionType.Injury:
                int Injury_dmg = Mathf.RoundToInt(duration); 
                character.TakeDamage(Injury_dmg);
                break;
            case ConditionType.Poisoned:
                int Poisoned_dmg = Mathf.RoundToInt(duration + 1);
                character.TakeDamage(Poisoned_dmg);
                break;
        }
    }

    public void OnExpire(Character character)
    {
        Debug.Log($"[Condition] {type} expired on {character.name}");
    }

    public float GetDodgeChance()
    {
        switch (type)
        {
            case ConditionType.Dodge:
                return 0.5f;
        }
        return 0f;
    }

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

    public float Get_Damage_Deal_Multiplier()
    {
        switch (type)
        {
            case ConditionType.Rage: return 1.25f;
            case ConditionType.Weakened: return 0.75f;
        }
        return 1f;
    }

    public float GetDamageTakenMultiplier()
    {
        switch (type)
        {
            case ConditionType.Vulnerable:
                return 1.5f;
        }
        return 1f;
    }

    public float GetShieldGainMultiplier()
    {
        switch (type)
        {
            case ConditionType.Fragile:
                return 0.5f; // chỉ nhận 50% shield
        }
        return 1f;
    }

    public bool ForceCritical()
    {
        switch (type)
        {
            case ConditionType.Mark:
                return true; 
            case ConditionType.Drunk:
                return Random.value < 0.5f; // 50% crit
        }
        return false;
    }

    public bool IsImmune()
    {
        return type == ConditionType.Immunity;
    }

}

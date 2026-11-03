using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [Header("HP")]
    public int maxHP = 0;
    public int currentHP = 0;
    public int spd = 0;

    [Header("Defense")]
    public int defense = 0;
    public int shield = 0;

    [Header("Condition Status")]
    public int maxConditions = 0;
    public int handSize = 0;

    [Header("Attack")]
    public float critChance = 0f;   
    public float critDamage = 1.5f;   
}

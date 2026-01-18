using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [Header("HP")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("Defense")]
    public int defense = 0;
    public int shield = 0;

    [Header("Attack")]
    public float critChance = 0f;   
    public float critDamage = 1.5f;   
}

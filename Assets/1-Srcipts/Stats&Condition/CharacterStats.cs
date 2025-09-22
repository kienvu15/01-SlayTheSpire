using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    public int maxHP = 100;
    public int currentHP = 100;

    public int defense = 0;

    public float critChance = 0f;   // 0%
    public float critDamage = 1.5f;   // 150%
}

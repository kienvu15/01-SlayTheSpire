using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnData
{
    public EnemyData enemyData;

    [Min(0)]
    public int slotIndex;
}

[Serializable]
public class WaveData
{
    public List<EnemySpawnData> enemies = new();
}

[CreateAssetMenu(
    fileName = "Encounter",
    menuName = "Enemy/Encounter"
)]
public class EncounterData : ScriptableObject
{
    [Header("Information")]
    public string encounterName;

    [Header("Spawn Chance")]
    [Min(0)] public float chance = 25f;

    [Header("Waves")]
    public WaveData[] waves;
}

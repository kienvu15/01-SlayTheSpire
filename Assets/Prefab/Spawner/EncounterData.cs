using UnityEngine;

[System.Serializable]
public class WaveData
{
    public EnemyType[] enemyTypes; // thay vì enemyPrefabs
}

[CreateAssetMenu(menuName = "Encounter/EncounterData")]
public class EncounterData : ScriptableObject
{
    public WaveData[] waves;
}

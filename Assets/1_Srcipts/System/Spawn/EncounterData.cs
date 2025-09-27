using UnityEngine;

[System.Serializable]
public class WaveData
{
    public EnemyType[] enemyTypes;
}

[CreateAssetMenu(menuName = "Encounter/EncounterData")]
public class EncounterData : ScriptableObject
{
    public WaveData[] waves;
}

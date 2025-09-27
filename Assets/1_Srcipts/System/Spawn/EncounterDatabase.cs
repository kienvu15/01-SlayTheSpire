using UnityEngine;

[CreateAssetMenu(fileName = "EncounterDatabase", menuName = "Database/EncounterDatabase")]
public class EncounterDatabase : ScriptableObject
{
    public EncounterData[] encounters;
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterDatabase", menuName = "Database/EncounterDatabase")]
public class EncounterDatabase : ScriptableObject
{
    public List<EncounterData> encounters = new List<EncounterData>();

    public EncounterData GetRandomEncounter()
    {
        if (encounters == null || encounters.Count == 0)
            return null;

        return encounters[Random.Range(0, encounters.Count)];
    }
}

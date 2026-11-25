
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MapEncounterDatabase",
    menuName = "Database/MapEncounterDatabase"
)]
public class MapEncounterDatabase : ScriptableObject
{
    [Header("First 3 Encounters")]
    public List<EncounterData> firstEncounters;

    [Header("Remaining Encounters")]
    public List<EncounterData> remainingEncounters;

    public EncounterData GetEncounter(int battleCount)
    {
        List<EncounterData> pool;

        // Battle 1, 2, 3
        if (battleCount <= 3)
            pool = firstEncounters;
        else
            pool = remainingEncounters;

        return GetWeightedEncounter(pool);
    }

    private EncounterData GetWeightedEncounter(
        List<EncounterData> encounters)
    {
        if (encounters == null || encounters.Count == 0)
            return null;

        float totalWeight = 0f;

        foreach (EncounterData encounter in encounters)
        {
            if (encounter != null)
                totalWeight += Mathf.Max(0f, encounter.chance);
        }

        if (totalWeight <= 0f)
            return null;

        float randomValue = Random.Range(0f, totalWeight);

        foreach (EncounterData encounter in encounters)
        {
            if (encounter == null)
                continue;

            randomValue -= Mathf.Max(0f, encounter.chance);

            if (randomValue < 0f)
                return encounter;
        }

        return null;
    }
}

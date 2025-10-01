using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterDatabase", menuName = "Database/EncounterDatabase")]
public class EncounterDatabase : ScriptableObject
{
    public List<EncounterData> earlyEncounters;  // dễ
    public List<EncounterData> midEncounters;    // vừa
    public List<EncounterData> lateEncounters;   // khó

    public EncounterData GetEncounterForStage(int battleCount)
    {
        if (battleCount < 4) 
            return GetRandomEncounter(earlyEncounters);
        else if (battleCount < 8) 
            return GetRandomEncounter(midEncounters);
        else // giai đoạn 3
            return GetRandomEncounter(lateEncounters);
    }

    private EncounterData GetRandomEncounter(List<EncounterData> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}

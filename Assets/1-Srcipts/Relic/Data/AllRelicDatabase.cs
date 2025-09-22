using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllRelicDatabase", menuName = "Relics/All Relic Database")]
public class AllRelicDatabase : ScriptableObject
{
    public List<Relic> allRelics = new List<Relic>();

    public Relic GetRandomRelic()
    {
        if (allRelics == null || allRelics.Count == 0) return null;
        int index = Random.Range(0, allRelics.Count);
        return allRelics[index];
    }
}

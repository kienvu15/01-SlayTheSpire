using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleCanvasDatabase", menuName = "Map/BattleCanvasDatabase")]
public class BattleCanvasDatabase : ScriptableObject
{
    public List<GameObject> battleCanvasPrefabs = new List<GameObject>();

    public GameObject GetRandomBattleCanvas()
    {
        if (battleCanvasPrefabs == null || battleCanvasPrefabs.Count == 0) return null;
        return battleCanvasPrefabs[Random.Range(0, battleCanvasPrefabs.Count)];
    }
}

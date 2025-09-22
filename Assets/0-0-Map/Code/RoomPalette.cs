using System.Collections.Generic;
using UnityEngine;

public enum RoomType 
{ 
    Start, 
    Battle,
    Elite,
    Event, 
    Shop, 
    Rest, 
    Boss 
}

[CreateAssetMenu(fileName = "RoomPalette", menuName = "Map/RoomPalette")]
public class RoomPalette : ScriptableObject
{
    [System.Serializable]
    public struct Entry { public RoomType type; public GameObject canvasPrefab; }

    public List<Entry> entries = new List<Entry>();

    public GameObject GetPrefabFor(RoomType type)
    {
        foreach (var e in entries) if (e.type == type) return e.canvasPrefab;
        return null;
    }
}
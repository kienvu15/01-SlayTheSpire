using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Enemy/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    [System.Serializable]
    public class EnemyEntry
    {
        public EnemyType type;
        public List<GameObject> prefabs; // nhiều prefab cho 1 type
    }

    public List<EnemyEntry> enemiesByType;

    public GameObject GetRandomPrefab(EnemyType type)
    {
        EnemyEntry entry = enemiesByType.Find(e => e.type == type);
        if (entry == null || entry.prefabs.Count == 0) return null;
        return entry.prefabs[Random.Range(0, entry.prefabs.Count)];
    }
}

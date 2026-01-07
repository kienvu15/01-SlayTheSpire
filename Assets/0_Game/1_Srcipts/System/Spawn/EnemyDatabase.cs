using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Enemy/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public List<GameObject> enemyPrefabs;

    public GameObject GetRandomPrefab()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return null;
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    }
}

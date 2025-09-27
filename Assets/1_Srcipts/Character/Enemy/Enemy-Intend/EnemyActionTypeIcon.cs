using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyActionTypeIcon", menuName = "Enemy/EnemyActionTypeIcon")]
public class EnemyActionTypeIcon : ScriptableObject
{
    [System.Serializable]
    public class TypeIconEntry
    {
        public Type type;
        public Sprite icon;
    }

    public List<TypeIconEntry> entries = new List<TypeIconEntry>();

    public Sprite GetIcon(Type type)
    {
        var entry = entries.Find(e => e.type == type);
        return entry != null ? entry.icon : null;
    }
}

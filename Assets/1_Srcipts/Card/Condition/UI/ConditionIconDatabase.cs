using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionIconDB", menuName = "UI/ConditionIconDatabase")]
public class ConditionIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class ConditionIconEntry
    {
        public ConditionType type;
        public Sprite icon;
        [TextArea]
        public string description; // thêm mô tả
    }

    public List<ConditionIconEntry> entries = new();

    public Sprite GetIcon(ConditionType type)
    {
        var entry = entries.Find(e => e.type == type);
        return entry != null ? entry.icon : null;
    }

    public string GetDescription(ConditionType type)
    {
        var entry = entries.Find(e => e.type == type);
        return entry != null ? entry.description : "No description.";
    }
}

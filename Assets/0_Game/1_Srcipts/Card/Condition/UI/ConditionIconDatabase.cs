using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionIconDB", menuName = "UI/ConditionIconDatabase")]
public class ConditionIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class ConditionIconEntry
    {
        public ConditionType type;
        public string displayName;
        public Sprite icon;
        [TextArea]
        public string description; 
    }

    public List<ConditionIconEntry> entries = new();
    private Dictionary<ConditionType, ConditionIconEntry> _lookup;

    private void BuildLookup()
    {
        _lookup = new Dictionary<ConditionType, ConditionIconEntry>();
        foreach (var e in entries)
            _lookup[e.type] = e;  
    }
    public bool TryGet(ConditionType type, out ConditionIconEntry entry)
    {
        if (_lookup == null) BuildLookup();
        return _lookup.TryGetValue(type, out entry);
    }

    public Sprite GetIcon(ConditionType type)
        => TryGet(type, out var e) ? e.icon : null;

    public string GetDescription(ConditionType type)
        => TryGet(type, out var e) ? e.description : "No description.";

    public string GetDisplayName(ConditionType type)
        => TryGet(type, out var e) && !string.IsNullOrEmpty(e.displayName)
            ? e.displayName
            : type.ToString();

    // Cập nhật lại lookup khi sửa data trong Inspector
    private void OnValidate() => _lookup = null;
}
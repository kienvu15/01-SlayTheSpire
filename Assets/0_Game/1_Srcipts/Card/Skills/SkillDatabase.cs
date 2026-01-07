using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Cards/Skill Database")]
public class SkillDataDatabase : ScriptableObject
{
    public List<SkillData> skills;

    private Dictionary<SkillType, SkillData> dict;

    public SkillData GetData(SkillType type)
    {
        if (dict == null || dict.Count == 0)
        {
            dict = new Dictionary<SkillType, SkillData>();
            foreach (var s in skills)
            {
                dict[s.type] = s;
            }
        }
        return dict.TryGetValue(type, out var data) ? data : null;
    }
}

[System.Serializable]
public class SkillData
{
    public SkillType type;
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;
}

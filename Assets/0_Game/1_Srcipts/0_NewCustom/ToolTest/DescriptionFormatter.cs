using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class DescriptionFormatter
{
    private static readonly Dictionary<ConditionType, string> ConditionColors = new()
    {
        { ConditionType.Dodge,     "#4FC3F7" },
        { ConditionType.Drunk,     "#FFB74D" },
        { ConditionType.Immunity,  "#FFD54F" },
        { ConditionType.Rage,      "#E53935" },
        { ConditionType.Blind,     "#9E9E9E" },
        { ConditionType.Fragile,   "#A1887F" },
        { ConditionType.Injury,    "#C62828" },
        { ConditionType.Mark,      "#BA68C8" },
        { ConditionType.Weakened,  "#81C784" },
    };
    private static Regex _regex;
    private static Regex GetRegex()
    {
        if (_regex != null) return _regex;
        string pattern = @"\b(" + string.Join("|", System.Enum.GetNames(typeof(ConditionType))) + @")\b";
        _regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        return _regex;
    }
    public static string Format(string description)
    {
        if (string.IsNullOrEmpty(description)) return description;

        return GetRegex().Replace(description, match =>
        {
            if (System.Enum.TryParse(match.Value, true, out ConditionType type)
                && ConditionColors.TryGetValue(type, out string hex))
            {
                return $"<color={hex}>{match.Value}</color>";
            }
            return match.Value;
        });
    }
    public static List<ConditionType> GetKeywords(string description)
    {
        var result = new List<ConditionType>();
        if (string.IsNullOrEmpty(description)) return result;

        foreach (Match m in GetRegex().Matches(description))
        {
            if (System.Enum.TryParse(m.Value, true, out ConditionType t) && !result.Contains(t))
                result.Add(t);
        }
        return result;
    }
    public static string GetColor(ConditionType type)
        => ConditionColors.TryGetValue(type, out var hex) ? hex : "#FFFFFF";
}
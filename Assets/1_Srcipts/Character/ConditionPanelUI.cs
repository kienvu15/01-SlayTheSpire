using System.Collections.Generic;
using UnityEngine;

public class ConditionPanelUI : MonoBehaviour
{
    [Header("References")]
    public GameObject conditionIconPrefab; // Prefab của icon
    public Transform container;             // Nơi chứa các icon

    [Header("Database")]
    public ConditionIconDatabase iconDatabase; // ScriptableObject chứa mapping type -> icon + mô tả

    private readonly List<ConditionIconUI> activeIcons = new();

    /// <summary>
    /// Cập nhật danh sách điều kiện đang có
    /// </summary>
    public void UpdateConditions(List<Condition> conditions)
    {
        // Clear cũ
        foreach (var icon in activeIcons)
        {
            Destroy(icon.gameObject);
        }
        activeIcons.Clear();

        // Add mới
        foreach (var cond in conditions)
        {
            var obj = Instantiate(conditionIconPrefab, container);
            var ui = obj.GetComponent<ConditionIconUI>();
            if (ui != null)
            {
                ui.Setup(cond.type, cond.duration, iconDatabase);
                activeIcons.Add(ui);
            }
        }
    }
}

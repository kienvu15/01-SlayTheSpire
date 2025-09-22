using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelUI : MonoBehaviour
{
    [Header("References")]
    public SkillDataDatabase database;   // asset chứa list skill
    public Transform skillContainer;     // nơi spawn icon
    public GameObject skillIconPrefab;   // prefab: Image + Text stack

    // lưu các icon UI theo type để update stack
    private readonly Dictionary<SkillType, SkillIconUI> activeIcons = new();



    // ------------------------
    // Thêm skill / hiển thị mới
    // ------------------------
    public void AddSkill(Skill skill)
    {
        if (skill == null) return;
        UpdateStacks(skill.type, skill.stacks);
    }

    // ------------------------
    // Cập nhật số stack (tạo nếu chưa có, xóa nếu stacks <= 0)
    // ------------------------
    public void UpdateStacks(SkillType type, int stacks)
    {
        // Nếu đã có icon -> set stack / xóa nếu cần
        if (activeIcons.TryGetValue(type, out var existingUi))
        {
            if (stacks <= 0)
            {
                Destroy(existingUi.gameObject);
                activeIcons.Remove(type);
            }
            else
            {
                existingUi.SetStacks(stacks);
            }
            return;
        }

        // Nếu chưa có và stacks > 0 -> tạo mới
        if (stacks > 0)
        {
            SkillData data = database != null ? database.GetData(type) : null;
            if (data == null)
            {
                Debug.LogWarning($"SkillPanelUI: No SkillData found for {type}");
                return;
            }

            GameObject go = Instantiate(skillIconPrefab, skillContainer);
            SkillIconUI ui = go.GetComponent<SkillIconUI>();
            if (ui == null)
            {
                Debug.LogError("SkillPanelUI: skillIconPrefab missing SkillIconUI component");
                Destroy(go);
                return;
            }

            ui.SetData(data, stacks);
            activeIcons[type] = ui;
        }
    }

    // ------------------------
    // Wrapper nhận Skill object
    // ------------------------
    public void UpdateSkill(Skill skill)
    {
        if (skill == null) return;
        UpdateStacks(skill.type, skill.stacks);
    }

    // ------------------------
    // Xóa skill UI theo type
    // ------------------------
    public void RemoveSkill(SkillType type)
    {
        if (activeIcons.TryGetValue(type, out var ui))
        {
            Destroy(ui.gameObject);
            activeIcons.Remove(type);
        }
    }

    // ------------------------
    // Tuỳ chọn: clear tất cả
    // ------------------------
    public void ClearAll()
    {
        foreach (var kv in activeIcons)
        {
            if (kv.Value != null) Destroy(kv.Value.gameObject);
        }
        activeIcons.Clear();
    }
}

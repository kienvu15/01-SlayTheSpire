using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillIconUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public TMP_Text stackText;

    private SkillData currentSkill;
    private static SkillIconUI currentlyOpen;

    [Header("Popup Panel")]
    public GameObject popupPanel;      // assign trong inspector
    public TMP_Text popupText;
    private CanvasGroup blockerGroup;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        // Tìm Block-Panel trong scene
        GameObject blocker = GameObject.Find("Block-Panel");
        if (blocker != null)
        {
            blockerGroup = blocker.GetComponent<CanvasGroup>();
            if (blockerGroup == null)
                blockerGroup = blocker.AddComponent<CanvasGroup>();

            HideBlocker(); // ẩn ngay từ đầu
        }
    }

    public void SetData(SkillData skill, int stacks)
    {
        currentSkill = skill;

        if (iconImage != null && skill != null)
            iconImage.sprite = skill.icon;

        SetStacks(stacks);
    }

    public void SetStacks(int stacks)
    {
        if (stackText != null)
            stackText.text = stacks.ToString();
    }

    public void OnClick()
    {
        if (popupPanel == null) return;
        Debug.Log("SkillIconUI: OnClick " + currentSkill?.displayName);

        // Nếu đang có panel khác mở → đóng nó
        if (currentlyOpen != null && currentlyOpen != this)
            currentlyOpen.HideDescription();

        bool newState = !popupPanel.activeSelf;
        popupPanel.SetActive(newState);

        if (newState)
        {
            currentlyOpen = this;

            if (popupText != null && currentSkill != null)
                popupText.text = $"<b>{currentSkill.displayName}</b>\n\n{currentSkill.description}";

            ShowBlocker();
        }
        else
        {
            currentlyOpen = null;
            HideBlocker();
        }
    }

    public void HideDescription()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (currentlyOpen == this)
            currentlyOpen = null;

        HideBlocker();
    }

    private void ShowBlocker()
    {
        if (blockerGroup != null)
        {
            blockerGroup.alpha = 0; // vẫn trong suốt
            blockerGroup.blocksRaycasts = true;  // chặn click
            blockerGroup.interactable = true;
        }
    }

    private void HideBlocker()
    {
        if (blockerGroup != null)
        {
            blockerGroup.blocksRaycasts = false; // cho click xuyên qua
            blockerGroup.interactable = false;
        }
    }
}

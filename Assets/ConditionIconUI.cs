using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConditionIconUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI durationText;

    private ConditionType currentType;
    private int currentDuration;
    private ConditionIconDatabase database;
    private static ConditionIconUI currentlyOpen;

    [Header("Popup Panel")]
    public GameObject popupPanel; // assign trong inspector
    private CanvasGroup blockerGroup;
    public TextMeshProUGUI popupText;

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

    public void Setup(ConditionType type, int duration, ConditionIconDatabase db)
    {
        currentType = type;
        currentDuration = duration;
        database = db;

        if (iconImage != null)
            iconImage.sprite = db.GetIcon(type);

        if (durationText != null)
            durationText.text = duration.ToString();
    }

    public void UpdateDuration(int newDuration)
    {
        currentDuration = newDuration;
        if (durationText != null)
            durationText.text = newDuration.ToString();
    }

    public void OnClick()
    {
        if (popupPanel == null) return;
        Debug.Log("ConditionIconUI: OnClick " + currentType);
        // Nếu đang có card khác mở → đóng nó
        if (currentlyOpen != null && currentlyOpen != this)
            currentlyOpen.HideDescription();

        bool newState = !popupPanel.activeSelf;
        popupPanel.SetActive(newState);

        if (newState)
        {
            currentlyOpen = this;
            if (popupText != null) popupText.text = database.GetDescription(currentType);

            ShowBlocker();
        }
        else
        {
            currentlyOpen = null;
            HideBlocker();
        }

     //   popupPanel.SetActive(true);
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

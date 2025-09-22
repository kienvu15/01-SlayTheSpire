using UnityEngine;
using UnityEngine.EventSystems;

public class OutsideClickCloser : MonoBehaviour, IPointerClickHandler
{
    void Start()
    {
        //gameObject.SetActive(false);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // Object mà chuột/tap đang trúng
        GameObject clickedObj = eventData.pointerCurrentRaycast.gameObject;

        // Tìm tất cả CardDisplay trong scene
        CardDisplay[] cards = FindObjectsByType<CardDisplay>(FindObjectsSortMode.None);

        foreach (var card in cards)
        {
            if (card.descriptionPanel == null) continue;
            if (!card.descriptionPanel.activeSelf) continue;

            // Nếu click vào chính panel hoặc card → bỏ qua
            if (clickedObj != null)
            {
                if (clickedObj.transform.IsChildOf(card.descriptionPanel.transform)) continue;
                if (clickedObj.transform.IsChildOf(card.transform)) continue;
            }

            // Ngược lại → click ngoài → ẩn
            card.HideDescription();
        }

        ConditionIconUI[] condIcons = FindObjectsByType<ConditionIconUI>(FindObjectsSortMode.None);
        foreach (var condIcon in condIcons)
        {
            if (condIcon.popupPanel == null) continue;
            if (!condIcon.popupPanel.activeSelf) continue;
            // Nếu click vào chính panel hoặc icon → bỏ qua
            if (clickedObj != null)
            {
                if (clickedObj.transform.IsChildOf(condIcon.popupPanel.transform)) continue;
                if (clickedObj.transform.IsChildOf(condIcon.transform)) continue;
            }
            // Ngược lại → click ngoài → ẩn
            condIcon.HideDescription();
        }

        SkillIconUI[] skillIcons = FindObjectsByType<SkillIconUI>(FindObjectsSortMode.None);
        foreach (var skillIcon in skillIcons)
        {
            if (skillIcon.popupPanel == null) continue;
            if (!skillIcon.popupPanel.activeSelf) continue;
            // Nếu click vào chính panel hoặc icon → bỏ qua
            if (clickedObj != null)
            {
                if (clickedObj.transform.IsChildOf(skillIcon.popupPanel.transform)) continue;
                if (clickedObj.transform.IsChildOf(skillIcon.transform)) continue;
            }
            // Ngược lại → click ngoài → ẩn
            skillIcon.HideDescription();
        }
    }
}

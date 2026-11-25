using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSelfCast : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public MidMatch_Manager match;

    [Header("Drop_Helper")]
    public Image cardDropHelper;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        CardDisplay cardDisplay = dropped.GetComponent<CardDisplay>();
        DraggableCard draggable = dropped.GetComponent<DraggableCard>();

        if (cardDisplay == null || cardDisplay.cardInstance == null || draggable == null)
        {
            Debug.LogError("[SelfCast] FAILED: Missing component or CardInstance");
            return;
        }

        CardData data = cardDisplay.cardInstance.data;

        // 1. Loại bài: SelfCast hoặc All đều thả được vào vùng này
        if (!(data.IsSelfCast() || data.IsAllCast()) || data.isUnplayable)
        {
            draggable.RevertToHand();
            return;
        }

        // 2. Điều kiện đánh bài (Requirement)
        if (!draggable.CanPlayCard(match.player))
        {
            draggable.RevertToHand();
            return;
        }

        // 3. Kiểm tra mana, trừ mana, rời tay, xếp hàng: tất cả nằm trong TryEnqueuePlay
        if (!ActionQueue.Instance.TryEnqueuePlay(cardDisplay, draggable, match.player))
        {
            draggable.RevertToHand();
            return;
        }

        // Tắt vùng thả self cast
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
            cg.alpha = 0;
        }
    }

}
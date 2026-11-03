using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;               
    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup;     
    private Transform originalBox;       
    public DraggableCard originalCard;

    [Header("References")]
    public CardDisplay cardDisplay;
    public CanvasGroup PlaySelfCast;
    private HandLayout handLayout;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvas = GetComponentInParent<Canvas>();
        PlaySelfCast = GameObject.Find("PlayerSelfCast")?.GetComponent<CanvasGroup>();

        Debug.Log($"[Drag] Awake: {name}");
        Debug.Log($"[Drag] Canvas: {canvas}");
        Debug.Log($"[Drag] CanvasGroup: {canvasGroup}");
        Debug.Log($"[Drag] PlaySelfCast: {PlaySelfCast}");
    }

    void Start()
    {
        BGHold bghold = GetComponentInParent<BGHold>();
        ShopSystem shopSystem = GetComponentInParent<ShopSystem>();

        if (bghold == null || shopSystem != null)
        {
            originalCard.enabled = false;
        }

        if (PlaySelfCast != null)
            PlaySelfCast.blocksRaycasts = false;

        Debug.Log($"[Drag] Start: {name} | BGHold: {bghold != null} | Shop: {shopSystem != null}");
    }


    private bool validDrop = false;

    public void MarkAsValidDrop()
    {
        validDrop = true;

        Debug.Log($"[Drag] MarkAsValidDrop: {name}");

        if (PlaySelfCast != null)
            PlaySelfCast.blocksRaycasts = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"[Drag] OnBeginDrag: {name}");

        validDrop = false;

        originalBox = transform.parent;

        handLayout = originalBox.GetComponent<HandLayout>();

        transform.SetParent(canvas.transform, true);

        canvasGroup.blocksRaycasts = false;

        if (cardDisplay != null)
        {
            CardData data = cardDisplay.GetCardData();

            if (data != null && data.IsSelfCast())
            {
                PlaySelfCast.blocksRaycasts = true;
                PlaySelfCast.alpha = 1;
            }
            else
            {
                PlaySelfCast.blocksRaycasts = false;
                PlaySelfCast.alpha = 0;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[Drag] OnEndDrag: {name} | ValidDrop: {validDrop}");

        if (!validDrop)
        {
            Debug.Log("[Drag] Drop FAILED -> Return to Hand");

            RevertToHand();
        }

        validDrop = false;

        canvasGroup.blocksRaycasts = true;

        PlaySelfCast.blocksRaycasts = false;
        PlaySelfCast.alpha = 0;
    }

    public void RevertToHand()
    {
        Debug.Log($"[Drag] RevertToHand: {name}");

        // Giữ nguyên vị trí thế giới khi đưa card về tay
        transform.SetParent(originalBox, true);

        // Khôi phục trạng thái card
        canvasGroup.blocksRaycasts = true;

        PlaySelfCast.blocksRaycasts = false;
        PlaySelfCast.alpha = 0;

        // Sắp xếp lại toàn bộ bài trên tay
        if (handLayout != null)
        {
            handLayout.RefreshLayout(true);
        }
        else
        {
            Debug.LogWarning("[Drag] HandLayout not found!");

            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }
    }

}

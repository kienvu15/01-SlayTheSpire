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
    [SerializeField] public CanvasGroup PlaySelfCast;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // tìm canvas cha gần nhất
        canvas = GetComponentInParent<Canvas>();
        PlaySelfCast = GameObject.Find("PlayerSelfCast")?.GetComponent<CanvasGroup>();
    }


    void Start()
    {
        BGHold bghold = GetComponentInParent<BGHold>();
        ShopSystem shopSystem = GetComponentInParent<ShopSystem>();
        if (bghold == null || shopSystem != null)
        {
            originalCard.enabled = false;
        }
        PlaySelfCast.blocksRaycasts = false;
    }

    private bool validDrop = false;

    public void MarkAsValidDrop()
    {
        validDrop = true;
        PlaySelfCast.blocksRaycasts = false;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        originalBox = transform.parent;
        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;

        if (cardDisplay != null)
        {
            CardData data = cardDisplay.GetCardData();
            if (data != null && data.IsSelfCast())
            {
                // nếu là self-cast thì BẬT chặn raycast
                PlaySelfCast.blocksRaycasts = true;
            }
            else
            {
                // tất cả card khác thì TẮT
                PlaySelfCast.blocksRaycasts = false;
                PlaySelfCast.alpha = 0;
            }
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        // cập nhật vị trí theo chuột (delta tính theo scale canvas)
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!validDrop)
        {
            // drop fail -> trả card về
            transform.SetParent(originalBox, false);
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }

        // reset flag
        validDrop = false;

        // bật lại raycast cho chính lá bài
        canvasGroup.blocksRaycasts = true;

        // reset vùng drop
        PlaySelfCast.blocksRaycasts = false;
        PlaySelfCast.alpha = 0;
    }



    public void RevertToHand()
    {
        transform.SetParent(originalBox, false);
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        canvasGroup.blocksRaycasts = true;
        PlaySelfCast.blocksRaycasts = false;
    }

}

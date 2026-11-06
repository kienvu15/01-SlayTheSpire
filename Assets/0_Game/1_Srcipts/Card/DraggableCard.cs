using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;               
    public DraggableCard originalCard;
    
    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup;     
    private Transform originalBox;
    private bool validDrop = false;

    [Header("References")]
    public CardDisplay cardDisplay;
    public CanvasGroup PlaySelfCast;
    private HandLayout handLayout;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        PlaySelfCast = GameObject.Find("PlayerSelfCast")?.GetComponent<CanvasGroup>();

        //Debug.Log($"[Drag] Awake: {name}");
        //Debug.Log($"[Drag] Canvas: {canvas}");
        //Debug.Log($"[Drag] CanvasGroup: {canvasGroup}");
        //Debug.Log($"[Drag] PlaySelfCast: {PlaySelfCast}");
    }

    void Start()
    {
        BGHold bghold = GetComponentInParent<BGHold>();
        ShopSystem shopSystem = GetComponentInParent<ShopSystem>();

        //if (bghold == null || shopSystem != null)
        //{
          //  originalCard.enabled = false;
       // }

        if (PlaySelfCast != null)
            PlaySelfCast.blocksRaycasts = false;

        Debug.Log($"[Drag] Start: {name} | BGHold: {bghold != null} | Shop: {shopSystem != null}");
    }

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

        transform.SetParent(originalBox, true);
        canvasGroup.blocksRaycasts = true;
        PlaySelfCast.blocksRaycasts = false;
        PlaySelfCast.alpha = 0;
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

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public DraggableCard originalCard;

    [Header("References")]
    public CardDisplay cardDisplay;
    public CanvasGroup PlaySelfCast;
    public PlayerSelfCast playSelfCast;
    private HandLayout handLayout;

    [Header("Routine")]
    private Coroutine manaCheckCoroutine;
    private float manaRevertDelay = 0.2f;

    private Canvas canvas;               
    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup;     
    private Transform originalBox;
    private bool validDrop = false;

    private void Awake()
    {
        cardDisplay = GetComponent<CardDisplay>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        playSelfCast = GameObject.Find("PlayerSelfCast")?.GetComponent<PlayerSelfCast>();
        PlaySelfCast = playSelfCast.GetComponent<CanvasGroup>();
    }

    void Start()
    {
        //BGHold bghold = GetComponentInParent<BGHold>();
        ShopSystem shopSystem = GetComponentInParent<ShopSystem>();

        if (PlaySelfCast != null)
            PlaySelfCast.blocksRaycasts = false;
    }

    private IEnumerator CheckManaWhileDragging()
    {
        yield return new WaitForSeconds(manaRevertDelay);

        if (!this.enabled || !gameObject.activeInHierarchy)
            yield break;
        if (cardDisplay == null || cardDisplay.cardInstance == null)
            yield break;

        if (!ManaSystem.instance.CanPlayCard(cardDisplay.cardInstance))
        {
            validDrop = false;

            RevertToHand();
            canvasGroup.blocksRaycasts = true;

            if (PlaySelfCast != null)
            {
                PlaySelfCast.blocksRaycasts = false;
                PlaySelfCast.alpha = 0;
            }

            playSelfCast.cardDropHelper.enabled = false;
        }

        manaCheckCoroutine = null;
    }

    public bool CanPlayCard(Character caster)
    {
        if (cardDisplay == null ||
            cardDisplay.cardInstance == null)
            return false;

        CardInstance currentCard = cardDisplay.cardInstance;

        List<CardInstance> remainingCards =
            Deck.instance.GetRemainingHandInstances(currentCard);

        RequirementContext ctx = new RequirementContext(
            caster,
            currentCard,
            remainingCards
        );

        bool canPlay = currentCard.data.CheckRequirements(
            ctx,
            out string failureMessage
        );

        if (!canPlay)
        {
            Debug.Log($"[Card Requirement] {failureMessage}");
            return false;
        }

        return true;
    }

    public void MarkAsValidDrop()
    {
        validDrop = true;
        if (PlaySelfCast != null)
            PlaySelfCast.blocksRaycasts = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        validDrop = false;

        originalBox = transform.parent;
        handLayout = originalBox.GetComponent<HandLayout>();

        if (handLayout != null)
            handLayout.PreviewDragCard(gameObject);

        transform.SetParent(canvas.transform, true);

        canvasGroup.blocksRaycasts = false;

        if (cardDisplay != null)
        {
            CardData data = cardDisplay.GetCardData();

            if (data != null && (data.IsSelfCast() || data.IsAllCast()))
                PlaySelfCast.blocksRaycasts = true;
            else
                PlaySelfCast.blocksRaycasts = false;
        }

        // Kiểm tra mana sau 0.5 giây
        if (manaCheckCoroutine != null)
            StopCoroutine(manaCheckCoroutine);

        if (!ManaSystem.instance.CanPlayCard(cardDisplay.cardInstance))
        {
            manaCheckCoroutine = StartCoroutine(CheckManaWhileDragging());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (manaCheckCoroutine != null)
        {
            StopCoroutine(manaCheckCoroutine);
            manaCheckCoroutine = null;
        }

        if (!validDrop)
        {
            RevertToHand();
            playSelfCast.cardDropHelper.enabled = false;
        }
        validDrop = false;
        canvasGroup.blocksRaycasts = true;
        PlaySelfCast.blocksRaycasts = false;
        PlaySelfCast.alpha = 0;
        playSelfCast.cardDropHelper.enabled = false;
    }

    public void RevertToHand()
    {
        transform.SetParent(originalBox, true);

        canvasGroup.blocksRaycasts = true;

        PlaySelfCast.blocksRaycasts = false;
        PlaySelfCast.alpha = 0;

        if (handLayout != null)
        {
            handLayout.ResetSortingOrder(gameObject);

            handLayout.SelectCardForce(gameObject);
            handLayout.RefreshLayout(true);
        }
        else
        {
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }
    }

}

using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public EnemyView enemy;
    public GameObject invalidImage;

    [Header("Drop State")]
    public bool isInvalidCardHover = false;
    private DraggableCard currentDraggable;

    [HideInInspector] public Discard discard;
    [HideInInspector] public Deck deck;
    [HideInInspector] public MidMatch_Manager match;
    [HideInInspector] public ManaSystem manaSystem;
    [HideInInspector] public CardPileViewer cardPileViewer;

    void Start()
    {
        match = FindAnyObjectByType<MidMatch_Manager>();
        manaSystem = FindAnyObjectByType<ManaSystem>();
        cardPileViewer = FindAnyObjectByType<CardPileViewer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject dragged = eventData.pointerDrag;
        if (dragged == null) return;

        DraggableCard draggable = dragged.GetComponent<DraggableCard>();
        if (draggable == null) return;

        CardDisplay display = dragged.GetComponent<CardDisplay>();
        if (display == null) return;

        CardData data = display.GetCardData();

        // Không phải SelfCast và cũng không phải OpponentCast
        isInvalidCardHover = data != null && !data.IsSelfCast();

        currentDraggable = draggable;
        invalidImage.SetActive(isInvalidCardHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInvalidCardHover = false;
        currentDraggable = null;
        invalidImage.SetActive(isInvalidCardHover);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        CardDisplay cardDisplay = dropped.GetComponent<CardDisplay>();
        DraggableCard draggable = dropped.GetComponent<DraggableCard>();
        if (cardDisplay == null || cardDisplay.cardInstance == null || draggable == null) return;

        CardData data = cardDisplay.cardInstance.data;

        if (!data.IsOpponentCast() || data.isUnplayable) return;

        if (!draggable.CanPlayCard(match.player))
        {
            draggable.RevertToHand();
            return;
        }

        invalidImage.SetActive(false);

        // Kiểm tra mana, trừ mana, rời tay, xếp hàng: tất cả nằm trong TryEnqueuePlay
        if (!ActionQueue.Instance.TryEnqueuePlay(cardDisplay, draggable, enemy))
            draggable.RevertToHand();
    }

}
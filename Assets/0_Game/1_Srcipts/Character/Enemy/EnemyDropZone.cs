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
        CardInstance card = cardDisplay.cardInstance;
        CardData data = card.data;
        // 1. Kiểm tra loại bài
        if (!data.IsOpponentCast() || data.isUnplayable)
        {
            //Debug.LogWarning($"{card.Name} không phải OpponentCast, không thể thả vào Enemy!");
            return;
        }
        // Kiểm tra điều kiện
        if (!draggable.CanPlayCard(match.player))
        {
            draggable.RevertToHand();
            return;
        }
        // 2. Kiểm tra mana (theo level)
        if (!manaSystem.CanPlayCard(card)) return;
        // 3. Trừ mana => valid drop
        invalidImage.SetActive(false);
        manaSystem.SpendMana(card.ManaCost);
        // 4. Apply effects theo level
        bool allSuccess = enemy != null && match != null &&
            CardEffectRunner.Run(
                card,
                match.player,
                enemy,
                manaSystem,
                deck,
                match.player,
                match.enemySystem.enemies,
                discard,
                cardPileViewer
            );
        // 5. Animation + discard
        if (allSuccess)
        {
            deck.RemoveCardFromHand(dropped, match);
            draggable.PlaySelfCast.blocksRaycasts = false;
            draggable.MarkAsValidDrop();
            RectTransform rect = dropped.GetComponent<RectTransform>();
            RectTransform discardPile = discard.GetComponent<RectTransform>();
            rect.SetParent(discardPile.parent, true);
            Vector3 startPos = rect.position;
            Vector3 endPos = discardPile.position;
            Vector3 midPos = (startPos + endPos) / 2f + Vector3.up * 9f;
            float flyTime = 0.7f;
            Sequence seq = DOTween.Sequence();
            // Bay lên nửa đầu, hơi thu nhỏ
            seq.Append(rect.DOScale(0.5f, flyTime * 0.5f).SetEase(Ease.OutSine));
            seq.Join(rect.DOMove(midPos, flyTime * 0.5f).SetEase(Ease.OutQuad));
            // Rơi xuống nửa sau, xoay
            Sequence fallSeq = DOTween.Sequence();
            fallSeq.Append(rect.DOMove(endPos, flyTime * 0.5f).SetEase(Ease.InCubic));
            fallSeq.Join(
                rect.DORotate(new Vector3(0, 0, 720f), flyTime * 0.5f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
            );
            seq.Append(fallSeq);
            // Biến mất nhanh khi gần chạm
            seq.Join(rect.DOScale(Vector3.zero, flyTime * 0.3f).SetEase(Ease.InBack).SetDelay(flyTime * 0.7f)
            );

            seq.OnComplete(() =>
            {
                discard.AddToDiscard(card);   // truyền CardInstance để giữ level
                Destroy(dropped);
            });
        }
        else
        {
            draggable.RevertToHand();
        }
    }
}
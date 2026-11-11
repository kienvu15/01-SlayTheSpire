using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerSelfCast : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public Discard discard;
    public Deck deck;
    public MidMatch_Manager match;
    public ManaSystem manaSystem;

    [Header("Drop_Helper")]
    public Image cardDropHelper;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        CardDisplay cardDisplay = dropped.GetComponent<CardDisplay>();
        DraggableCard draggable = dropped.GetComponent<DraggableCard>();

        // Kiểm tra null TRƯỚC khi đọc cardInstance
        if (cardDisplay == null || cardDisplay.cardInstance == null || draggable == null)
        {
            Debug.LogError("[SelfCast] FAILED: Missing component or CardInstance");
            return;
        }

        CardInstance card = cardDisplay.cardInstance;
        CardData data = card.data;

        Debug.Log($"[SelfCast] {card.Name} (level {card.level}) | IsSelfCast: {data.IsSelfCast()}");

        // 1. Kiểm tra loại bài (SelfCast hoặc All đều thả được vào vùng này)
        bool acceptable = data.IsSelfCast() || data.IsAllCast();
        if (!acceptable || data.isUnplayable)
        {
            Debug.LogWarning("[SelfCast] FAILED: Card is not SelfCast/All");
            draggable.RevertToHand();
            return;
        }

        // 2. Kiểm tra mana (theo mana sau nâng cấp)
        if (!manaSystem.CanPlayCard(card))
        {
            //Debug.LogWarning("[SelfCast] FAILED: Not enough Mana");
            draggable.RevertToHand();
            return;
        }

        // 3. Trừ mana
        manaSystem.SpendMana(card.ManaCost);

        // 4. Apply effects theo level
        bool allSuccess = false;
        if (match != null)
        {
            allSuccess = CardEffectRunner.Run(
                card, match.player, match.player, manaSystem, deck,
                match.player, match.enemySystem.enemies
            );
        }
        else
        {
            Debug.LogError("[SelfCast] match is NULL");
        }

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
            seq.Append(rect.DOScale(0.5f, flyTime * 0.5f).SetEase(Ease.OutSine));
            seq.Join(rect.DOMove(midPos, flyTime * 0.5f).SetEase(Ease.OutQuad));

            Sequence fallSeq = DOTween.Sequence();
            fallSeq.Append(rect.DOMove(endPos, flyTime * 0.5f).SetEase(Ease.InCubic));
            fallSeq.Join(
                rect.DORotate(new Vector3(0, 0, 720f), flyTime * 0.5f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
            );
            seq.Append(fallSeq);

            seq.Join(
                rect.DOScale(Vector3.zero, flyTime * 0.3f)
                    .SetEase(Ease.InBack)
                    .SetDelay(flyTime * 0.7f)
            );

            seq.OnComplete(() =>
            {
                discard.AddToDiscard(card);   // truyền CardInstance để giữ level
                Destroy(dropped);
            });
        }
        else
        {
            Debug.LogError("[SelfCast] FAILED: One or more effects returned false");
            draggable.RevertToHand();
        }

        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
            cg.alpha = 0;
        }
    }

}
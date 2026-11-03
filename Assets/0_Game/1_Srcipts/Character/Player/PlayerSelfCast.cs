using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSelfCast : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public Discard discard;
    public Deck deck;
    public MidMatch_Manager match;
    public ManaSystem manaSystem;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("========== SELF CAST ON DROP ==========");

        GameObject dropped = eventData.pointerDrag;

        Debug.Log($"[SelfCast] PointerDrag: {dropped}");

        if (dropped == null)
        {
            Debug.LogError("[SelfCast] FAILED: PointerDrag is NULL");
            return;
        }

        CardDisplay cardDisplay = dropped.GetComponent<CardDisplay>();
        DraggableCard draggable = dropped.GetComponent<DraggableCard>();

        Debug.Log($"[SelfCast] CardDisplay: {cardDisplay}");
        Debug.Log($"[SelfCast] Draggable: {draggable}");

        if (cardDisplay == null || cardDisplay.cardData == null || draggable == null)
        {
            Debug.LogError("[SelfCast] FAILED: Missing component or CardData");
            return;
        }

        CardData card = cardDisplay.cardData;

        Debug.Log($"[SelfCast] Card Name: {card.cardName}");
        Debug.Log($"[SelfCast] IsSelfCast: {card.IsSelfCast()}");

        // 1. Kiểm tra loại Card
        if (!card.IsSelfCast())
        {
            Debug.LogWarning("[SelfCast] FAILED: Card is NOT SelfCast");

            draggable.RevertToHand();
            return;
        }

        Debug.Log("[SelfCast] PASS: Card type");

        // 2. Kiểm tra Mana
        Debug.Log($"[SelfCast] Mana check: {manaSystem.CanPlayCard(card)}");

        if (!manaSystem.CanPlayCard(card))
        {
            Debug.LogWarning("[SelfCast] FAILED: Not enough Mana");

            draggable.RevertToHand();
            return;
        }

        Debug.Log("[SelfCast] PASS: Mana");

        // 3. Trừ Mana
        manaSystem.SpendMana(card.manaCost);

        Debug.Log($"[SelfCast] Mana spent: {card.manaCost}");

        bool allSuccess = true;

        // 4. Apply effects
        if (match != null && card.effects != null)
        {
            Debug.Log($"[SelfCast] Effects count: {card.effects}");

            int index = 0;

            foreach (var wrapper in card.effects)
            {
                Debug.Log($"[SelfCast] Processing effect #{index}");

                if (wrapper == null || wrapper.effect == null)
                {
                    Debug.LogWarning($"[SelfCast] Effect #{index} is NULL");
                    index++;
                    continue;
                }

                bool success = true;

                if (wrapper.overrideValue && wrapper.effect is IOverrideValue overridable)
                {
                    Debug.Log($"[SelfCast] Effect #{index}: Override");

                    overridable.ApplyWithOverride(
                        match.player,
                        match.player,
                        wrapper.valueOverride
                    );
                }
                else
                {
                    Debug.Log($"[SelfCast] Effect #{index}: Apply");

                    success = wrapper.effect.Apply(
                        match.player,
                        match.player,
                        manaSystem,
                        deck
                    );
                }

                Debug.Log($"[SelfCast] Effect #{index} Success: {success}");

                if (!success)
                    allSuccess = false;

                index++;
            }
        }
        else
        {
            Debug.LogError("[SelfCast] match OR card.effects is NULL");
        }

        Debug.Log($"[SelfCast] AllSuccess: {allSuccess}");

        if (allSuccess)
        {
            Debug.Log("[SelfCast] SUCCESS -> Play card animation");

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
                rect.DORotate(
                    new Vector3(0, 0, 720f),
                    flyTime * 0.5f,
                    RotateMode.FastBeyond360
                ).SetEase(Ease.Linear)
            );

            seq.Append(fallSeq);

            seq.Join(
                rect.DOScale(Vector3.zero, flyTime * 0.3f)
                .SetEase(Ease.InBack)
                .SetDelay(flyTime * 0.7f)
            );

            seq.OnComplete(() =>
            {
                Debug.Log("[SelfCast] Animation completed -> AddToDiscard");

                discard.AddToDiscard(card);
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

        Debug.Log("========== SELF CAST END ==========");
    }

}
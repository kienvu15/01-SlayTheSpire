using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDropZone : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public Enemy enemy;     
    public Discard discard;
    public Deck deck;
    public Match match;
    public ManaSystem manaSystem;

    void Start()
    {
        
        match = FindFirstObjectByType<Match>();
        manaSystem = FindFirstObjectByType<ManaSystem>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        CardDisplay cardDisplay = dropped.GetComponent<CardDisplay>();
        DraggableCard draggable = dropped.GetComponent<DraggableCard>();
        if (cardDisplay == null || cardDisplay.cardData == null || draggable == null) return;

        var card = cardDisplay.cardData;

        if (!card.IsOpponentCast())
        {
            Debug.LogWarning($"{card.cardName} không phải OpponentCast, không thể thả vào Enemy!");
            return;
        }

        if (!manaSystem.CanPlayCard(card)) return;

        manaSystem.SpendMana(card.manaCost);

        bool allSuccess = true;

        if (enemy != null && card.effects != null)
        {
            foreach (var wrapper in card.effects)
            {
                if (wrapper == null || wrapper.effect == null) continue;
                Debug.Log($"[EnemyDropZone] Running effect {wrapper.effect.name} on {enemy.name}");

                bool success = true;

                if (wrapper.overrideValue && wrapper.effect is IOverrideValue overridable)
                {
                    overridable.ApplyWithOverride(match.player, enemy, wrapper.valueOverride);
                }
                else
                {
                    success = wrapper.effect.Apply(match.player, enemy, manaSystem, deck);
                }

                if (!success)
                {
                    allSuccess = false;
                }

            }
        }


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

            // Cung bay nhẹ (thấp)
            Vector3 midPos = (startPos + endPos) / 2f + Vector3.up * 9f;

            float flyTime = 0.7f;

            Sequence seq = DOTween.Sequence();

            // 1️⃣ Bay lên mượt nửa đầu, hơi thu nhỏ
            seq.Append(rect.DOScale(0.5f, flyTime * 0.5f).SetEase(Ease.OutSine));
            seq.Join(rect.DOMove(midPos, flyTime * 0.5f).SetEase(Ease.OutQuad));

            // 2️⃣ Rơi xuống nửa sau, xoay ngay khi bắt đầu rơi
            Sequence fallSeq = DOTween.Sequence();
            fallSeq.Append(rect.DOMove(endPos, flyTime * 0.5f).SetEase(Ease.InCubic));
            fallSeq.Join(
                rect.DORotate(new Vector3(0, 0, 720f), flyTime * 0.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear) 
            );
            seq.Append(fallSeq);

            // 3️⃣ Biến mất nhanh khi gần chạm
            seq.Join(
                rect.DOScale(Vector3.zero, flyTime * 0.3f)
                .SetEase(Ease.InBack)
                .SetDelay(flyTime * 0.7f)
            );

            seq.OnComplete(() =>
            {
            discard.AddToDiscard(card);
                Destroy(dropped);
            });
        }
        else
        {
            draggable.RevertToHand();
        }





    }

}

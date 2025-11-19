using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSelfCast : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public Discard discard;
    public Deck deck;
    public Match match;
    public ManaSystem manaSystem;

    void Start()
    {


    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return;

        CardDisplay cardDisplay = dropped.GetComponent<CardDisplay>();
        DraggableCard draggable = dropped.GetComponent<DraggableCard>();
        if (cardDisplay == null || cardDisplay.cardData == null || draggable == null) return;

        CardData card = cardDisplay.cardData;

        // 1. Kiểm tra loại Card
        if (!card.IsSelfCast())
        {
            Debug.LogWarning($"{card.cardName} không phải SelfCast, không thể thả vào Player!");
            // Vẫn phải gọi RevertToHand() vì người chơi đã nhấc thẻ ra khỏi vị trí ban đầu
            draggable.RevertToHand();
            return;
        }

        // 2. Kiểm tra Mana
        if (!manaSystem.CanPlayCard(card))
        {
            draggable.RevertToHand();
            return;
        }

        // 3. Trừ Mana
        manaSystem.SpendMana(card.manaCost);

        bool allSuccess = true;

        // 4. Gọi PlayCard (tác dụng)
        if (match != null && card.effects != null)
        {
            foreach (var wrapper in card.effects)
            {
                if (wrapper == null || wrapper.effect == null) continue;

                bool success = true;

                if (wrapper.overrideValue && wrapper.effect is IOverrideValue overridable)
                {
                    // Tác dụng lên Player (người chơi)
                    overridable.ApplyWithOverride(match.player, match.player, wrapper.valueOverride);
                }
                else
                {
                    // Tác dụng lên Player (người chơi)
                    success = wrapper.effect.Apply(match.player, match.player, manaSystem, deck);
                }

                if (!success)
                {
                    allSuccess = false;
                }

                // Nếu Apply thành công → hiện hiệu ứng (Có thể bỏ comment dòng dưới nếu cần)
                // if (success)
                // {
                //    AttackImpactManager.Instance.ShowImpact(card.cardType, match.player.transform);
                // }
            }
        }


        if (allSuccess)
        {
            // 5. Xử lý UI và Animation bay vào Discard (giống EnemyDropZone)
            deck.RemoveCardFromHand(dropped, match);
            draggable.PlaySelfCast.blocksRaycasts = false;
            draggable.MarkAsValidDrop();

            RectTransform rect = dropped.GetComponent<RectTransform>();
            RectTransform discardPile = discard.GetComponent<RectTransform>();

            // Chuyển card lên cấp cao hơn để animation không bị cắt
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
                // 6. Thêm card vào discard và xóa game object sau khi animation hoàn tất
                discard.AddToDiscard(card);
                Destroy(dropped);
            });
        }
        else
        {
            // Nếu có effect nào đó thất bại, hoàn trả thẻ về tay
            draggable.RevertToHand();
        }

        // 7. Sau khi xử lý xong, luôn reset drop zone (tuy nhiên, với logic animation, 
        // bạn có thể muốn giữ lại cái này hoặc di chuyển nó vào logic sau khi animation/revert,
        // nhưng để giống EnemyDropZone, tôi giữ lại RevertToHand() ở else.)
        // Do PlayerSelfCast này được tham chiếu trong DraggableCard, ta có thể reset nó ở đây.
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
            cg.alpha = 0;
        }
    }
}
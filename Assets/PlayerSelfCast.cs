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
        if (cardDisplay == null || cardDisplay.cardData == null) return;

        CardData card = cardDisplay.cardData;
        DraggableCard draggable = dropped.GetComponent<DraggableCard>();

        if (manaSystem.CanPlayCard(card))
        {
            if (!card.IsSelfCast())
            {
                Debug.LogWarning($"{card.cardName} không phải SelfCast!");
                return;
            }

            // Trừ mana
            manaSystem.SpendMana(card.manaCost);

            // 👉 Gọi PlayCard (tác dụng)
            if (match != null)
            {
                foreach (var wrapper in card.effects)
                {
                    if (wrapper == null || wrapper.effect == null) continue;
                    bool success = true;
                    if (wrapper.overrideValue && wrapper.effect is IOverrideValue overridable)
                    {
                        overridable.ApplyWithOverride(match.player, match.player, wrapper.valueOverride);
                    }
                    else
                    {
                        success = wrapper.effect.Apply(match.player, match.player, manaSystem, deck);
                    }

                    // Nếu Apply thành công → hiện hiệu ứng
                    if (success)
                    {
                       // AttackImpactManager.Instance.ShowImpact(card.cardType, match.player.transform);
                    }
                }
            }

            // Thêm card vào discard
            discard.AddToDiscard(card);

            // Xóa card khỏi tay
            deck.RemoveCardFromHand(dropped, match);
        }

        // 🚀 Sau khi xử lý xong, luôn reset drop zone
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.blocksRaycasts = false;
            cg.alpha = 0;
        }
    }

}

using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDropZone : MonoBehaviour, IDropHandler
{
    [Header("References")]
    public Enemy enemy;         // Kéo Enemy gắn vào panel
    public Discard discard;
    public Deck deck;
    public Match match;
    public ManaSystem manaSystem;

    void Start()
    {
        discard = FindFirstObjectByType<Discard>();
        deck = FindFirstObjectByType<Deck>();
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

        // Chỉ chấp nhận card OpponentCast
        if (!card.IsOpponentCast())
        {
            Debug.LogWarning($"{card.cardName} không phải OpponentCast, không thể thả vào Enemy!");
            return;
        }

        // Check mana
        if (!manaSystem.CanPlayCard(card)) return;

        // Trừ mana
        manaSystem.SpendMana(card.manaCost);

        bool allSuccess = true;

        // 👉 Apply effect (attack value, custom EffectData…)
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

                if (success)
                {
//                    AttackImpactManager.Instance.ShowImpact(card.cardType, enemy.transform);
                }
                else
                {
                    allSuccess = false;
                }

            }
        }


        if (allSuccess)
        {
            // Bỏ card vào discard
            discard.AddToDiscard(card);

            // Xóa card khỏi tay
            deck.RemoveCardFromHand(dropped, match);
            draggable.PlaySelfCast.blocksRaycasts  = false;
            // ✅ Đánh dấu drop thành công → không bị revert về tay
            draggable.MarkAsValidDrop();
        }
        else
        {
            // ❌ Nếu effect fail → trả card về tay
            draggable.RevertToHand();
        }
    }

}

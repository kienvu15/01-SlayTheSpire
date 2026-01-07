using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardEffect", menuName = "Cards/Effects/Draw Card")]
public class DrawCardEffect : EffectData
{
    public int amount = 1;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (deck == null) return false;

        // Nếu deck + discard đều trống => không rút được gì
        if (deck.GetDeckCount() == 0 && deck.discard.Count == 0)
        {
            Debug.Log("[DrawCardEffect] No cards in deck and discard!");
            return false;
        }

        // Nếu deck rỗng nhưng discard còn => refill trước
        if (deck.GetDeckCount() == 0 && deck.discard.Count > 0)
        {
            deck.StartCoroutine(RefillAndDraw(deck, amount));
        }
        else
        {
            deck.StartCoroutine(deck.DrawCards(amount));
        }

        return true;
    }

    private System.Collections.IEnumerator RefillAndDraw(Deck deck, int count)
    {
        Debug.Log("[DrawCardEffect] Deck empty, refilling from discard...");

        // chờ 1 chút cho hiệu ứng refill
        yield return new WaitForSeconds(deck.refillDelay);

        // lấy toàn bộ discard đưa vào deck
        List<CardData> discardCards = deck.discard.TakeAllCards();
        deck.ReceiveCards(discardCards);
        deck.ShuffleDeck();

        // sau khi refill thì rút tiếp
        yield return deck.StartCoroutine(deck.DrawCards(count));
    }
}

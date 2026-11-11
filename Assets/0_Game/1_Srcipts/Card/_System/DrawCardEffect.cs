using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Draw Card")]
public class DrawCardEffect : EffectData
{
    public int amount = 1;

    public override bool Apply(EffectContext ctx)
    {
        var deck = ctx.deck;
        if (deck == null) return false;
        int count = ctx.GetValue(amount);

        if (deck.GetDeckCount() == 0 && deck.discard.Count == 0) return false;

        if (deck.GetDeckCount() == 0)
            deck.StartCoroutine(RefillAndDraw(deck, count));
        else
            deck.StartCoroutine(deck.DrawCards(count));
        return true;
    }

    private System.Collections.IEnumerator RefillAndDraw(Deck deck, int count)
    {
        yield return new WaitForSeconds(deck.refillDelay);
        deck.ReceiveCards(deck.discard.TakeAllCards());
        deck.ShuffleDeck();
        yield return deck.StartCoroutine(deck.DrawCards(count));
    }
}
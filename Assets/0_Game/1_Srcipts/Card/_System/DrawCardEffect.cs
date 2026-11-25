using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Draw Card")]
public class DrawCardEffect : EffectData
{
    public int amount = 1;

    public override IEnumerator ApplyRoutine(EffectContext ctx, System.Action<bool> done)
    {
        var deck = ctx.deck;
        if (deck == null) { done(false); yield break; }
        int need = Mathf.Max(1, ctx.GetValue(amount));
        if (deck.GetDeckCount() == 0 && deck.discard.Count == 0) { done(false); yield break; }
        while (need > 0)
        {
            int available = deck.GetDeckCount();
            if (available > 0)
            {
                int drawNow = Mathf.Min(available, need);
                yield return deck.DrawCards(drawNow);
                need -= drawNow;
            }
            else if (deck.discard.Count > 0)
            {
                yield return new WaitForSeconds(deck.refillDelay);
                deck.ReceiveCards(deck.discard.TakeAllCards());
                deck.ShuffleDeck();
            }
            else break;
        }
        done(true);
    }

    public override bool Apply(EffectContext ctx)
    {
        if (ctx.deck == null) return false;
        ctx.deck.StartCoroutine(ApplyRoutine(ctx, _ => { }));
        return true;
    }
}
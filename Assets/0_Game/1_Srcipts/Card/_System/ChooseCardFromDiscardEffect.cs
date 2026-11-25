using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/ChooseCardFromDiscardEffect")]
public class ChooseCardFromDiscardEffect : EffectData
{
    public int amount = 1;   // override value = số lá được chọn

    public override IEnumerator ApplyRoutine(EffectContext ctx, System.Action<bool> done)
    {
        Deck deck = ctx.deck;
        if (deck == null || deck.discard == null || deck.pileViewer == null) { done(false); yield break; }
        if (deck.discard.Count <= 0) { done(true); yield break; }
        if (ctx.enemies != null && !ctx.enemies.Exists(e => e != null && e.stats.currentHP > 0)) { done(true); yield break; }

        int count = Mathf.Max(1, ctx.GetValue(amount));
        bool finished = false;
        List<CardInstance> result = null;

        deck.pileViewer.BeginSelect(CardPileType.DiscardPile, count, picked => { result = picked; finished = true; });
        yield return new WaitUntil(() => finished);

        yield return deck.MoveDiscardToHand(result);   // xem Deck bên dưới: giờ trả IEnumerator
        done(true);
    }

    public override bool Apply(EffectContext ctx) => true;

    //public override bool Apply(EffectContext ctx)
    //{
    //    Deck deck = ctx.deck;
    //    if (deck == null || deck.discard == null || deck.pileViewer == null)
    //        return false;

    //    if (deck.discard.Count <= 0)
    //        return true;

    //    // Hết enemy (vừa đánh chết con cuối): không mở panel chọn bài nữa
    //    if (ctx.enemies != null && !ctx.enemies.Exists(e => e != null && e.stats.currentHP > 0))
    //        return true;

    //    int count = Mathf.Max(1, ctx.GetValue(amount));

    //    GameStage.Instance.SetBusy(true);

    //    deck.pileViewer.BeginSelect(CardPileType.DiscardPile, count, picked =>
    //    {
    //        deck.MoveDiscardToHand(picked);
    //        GameStage.Instance.SetBusy(false);
    //    });

    //    return true;
    //}

}
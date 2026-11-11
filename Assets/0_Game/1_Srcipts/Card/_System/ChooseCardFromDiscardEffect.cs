using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/ChooseCardFromDiscardEffect")]
public class ChooseCardFromDiscardEffect : EffectData
{
    public int amount = 1;   // override value = số lá được chọn

    public override bool Apply(EffectContext ctx)
    {
        Deck deck = ctx.deck;
        if (deck == null || deck.discard == null || deck.pileViewer == null)
            return false;

        if (deck.discard.Count <= 0)
            return true;

        // Hết enemy (vừa đánh chết con cuối): không mở panel chọn bài nữa
        if (ctx.enemies != null && !ctx.enemies.Exists(e => e != null && e.stats.currentHP > 0))
            return true;

        int count = Mathf.Max(1, ctx.GetValue(amount));

        GameStage.Instance.SetBusy(true);

        deck.pileViewer.BeginSelect(CardPileType.DiscardPile, count, picked =>
        {
            deck.MoveDiscardToHand(picked);
            GameStage.Instance.SetBusy(false);
        });

        return true;
    }

}
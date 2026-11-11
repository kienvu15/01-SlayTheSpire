using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Upgrade Other Cards")]
public class UpgradeCardsEffect : EffectData
{
    [Header("Số lá nâng cấp")]
    public int handCount = 1;       
    public int drawPileCount = 1;

    public override bool Apply(EffectContext ctx)
    {
        if (ctx.deck == null) return false;

        int hand = ctx.GetValue(handCount);

        if (hand > 0)
            ctx.deck.UpgradeRandomInHand(hand, ctx.sourceCard);
        if (drawPileCount > 0)
            ctx.deck.UpgradeRandomInDrawPile(drawPileCount);

        return true; 
    }
}
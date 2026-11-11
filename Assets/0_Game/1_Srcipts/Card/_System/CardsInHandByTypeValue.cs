using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Values/Cards In Hand By Type")]
public class CardsInHandByTypeValue : ValueSourceData
{
    public CardType cardType = CardType.Mellee;
    public bool excludeSourceCard = true;   // không đếm chính lá đang đánh

    public override int GetValue(EffectContext ctx)
    {
        if (ctx.deck == null) return 0;

        var cards = ctx.deck.GetHandCards(excludeSourceCard ? ctx.sourceCard : null);

        int count = 0;
        foreach (var c in cards)
            if (c.data.cardType == cardType) count++;
        return count;
    }
}
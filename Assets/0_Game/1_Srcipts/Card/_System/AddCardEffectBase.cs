using UnityEngine;

public abstract class AddCardEffectBase : EffectData
{
    [Header("Card")]
    public CardData cardToAdd;
    public int startLevel = 1;      

    [Header("Amount")]
    public int amount = 1;          

    [Header("Options")]
    public bool isTemporary = true; 

    protected abstract void AddOne(Deck deck, CardInstance card);

    public override bool Apply(EffectContext ctx)
    {
        Deck deck = ctx.deck;
        if (deck == null || deck.discard == null || deck.pileViewer == null)
            return false;                       // thiếu reference là lỗi setup

        if (deck.discard.Count <= 0)
            return true;                        // không có gì để chọn: bỏ qua, lá vẫn được đánh

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
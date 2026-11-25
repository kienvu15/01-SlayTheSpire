using UnityEngine;

public abstract class AddCardEffectBase : EffectData
{
    [Header("Card")]
    public CardData cardToAdd;
    public int startLevel = 1;

    [Header("Amount")]
    public int amount = 1;          // override value = số bản sao

    [Header("Options")]
    public bool isTemporary = true; // mặc định là bài tạm

    protected abstract void AddOne(Deck deck, CardInstance card);

    public override bool Apply(EffectContext ctx)
    {
        if (ctx.deck == null || cardToAdd == null) return false;

        int count = Mathf.Max(1, ctx.GetValue(amount));
        for (int i = 0; i < count; i++)
            AddOne(ctx.deck, new CardInstance(cardToAdd, startLevel));   // mỗi bản sao là một instance riêng

        return true;
    }
}
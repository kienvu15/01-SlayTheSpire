using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Add Card To Draw Pile")]
public class AddCardToDrawPileEffect : AddCardEffectBase
{
    public DrawPileInsert insertPosition = DrawPileInsert.Random;

    protected override void AddOne(Deck deck, CardInstance card)
        => deck.AddCardToDrawPile(card, insertPosition, isTemporary);
}
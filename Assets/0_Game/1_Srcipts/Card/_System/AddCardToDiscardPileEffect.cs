using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Add Card To Discard Pile")]
public class AddCardToDiscardPileEffect : AddCardEffectBase
{
    protected override void AddOne(Deck deck, CardInstance card)
        => deck.AddCardToDiscardPile(card, isTemporary);
}
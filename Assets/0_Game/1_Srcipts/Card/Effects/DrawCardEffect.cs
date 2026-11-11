//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "DrawCardEffect", menuName = "Cards/Effects/Draw Card")]
//public class DrawCardEffect : EffectData, IOverrideValue
//{
//    public int amount = 1;

//    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
//        => DoDraw(deck, amount);

//    public void ApplyWithOverride(Character self, Character target, int overrideValue)
//        => DoDraw(Deck.instance, overrideValue);

//    public override int GetIntentValue() => amount;
//    public int GetIntentValue(int? overrideValue = null) => overrideValue ?? amount;

//    private bool DoDraw(Deck deck, int count)
//    {
//        if (deck == null) return false;

//        if (deck.GetDeckCount() == 0 && deck.discard.Count == 0)
//        {
//            Debug.Log("[DrawCardEffect] No cards in deck and discard!");
//            return false;
//        }

//        if (deck.GetDeckCount() == 0)
//            deck.StartCoroutine(RefillAndDraw(deck, count));
//        else
//            deck.StartCoroutine(deck.DrawCards(count));

//        return true;
//    }

//    private System.Collections.IEnumerator RefillAndDraw(Deck deck, int count)
//    {
//        Debug.Log("[DrawCardEffect] Deck empty, refilling from discard...");

//        yield return new WaitForSeconds(deck.refillDelay);

//        List<CardInstance> discardCards = deck.discard.TakeAllCards();
//        deck.ReceiveCards(discardCards);
//        deck.ShuffleDeck();

//        yield return deck.StartCoroutine(deck.DrawCards(count));
//    }
//}
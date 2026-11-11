//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "AttackAndDrawIfEmpty", menuName = "Cards/Effects/AttackAndDrawIfEmpty")]
//public class AttackAndDrawIfEmptyEffect : EffectData, IOverrideValue
//{
//    public int damage = 7;
//    public CardType vfxType = CardType.Mellee;
//    public int drawIfHandEmpty = 1;

//    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
//    {
//        return DoAttack(self, target, deck, damage);
//    }

//    public void ApplyWithOverride(Character self, Character target, int overrideValue)
//    {
//        // ApplyWithOverride không nhận deck nên lấy từ singleton
//        DoAttack(self, target, Deck.instance, overrideValue);
//    }

//    // Logic chung, không sửa field của ScriptableObject
//    private bool DoAttack(Character self, Character target, Deck deck, int dmg)
//    {
//        if (target == null) return false;

//        bool hit = self.DealDamage(target, dmg, vfxType);

//        if (hit && deck != null)
//        {
//            // Chờ 1 frame cho drop zone xóa card khỏi hand rồi mới kiểm tra
//            self.StartCoroutine(DelayedCheck(deck));
//        }

//        return hit;
//    }

//    private IEnumerator DelayedCheck(Deck deck)
//    {
//        yield return null;

//        if (deck.GetCurrentHandObjects().Count != 0) yield break;

//        Debug.Log($"[AttackAndDrawIfEmptyEffect] Hand empty, draw {drawIfHandEmpty} cards");

//        if (deck.GetDeckCount() == 0 && deck.discard.Count == 0)
//        {
//            Debug.Log("[AttackAndDrawIfEmptyEffect] No cards in deck and discard!");
//            yield break;
//        }

//        if (deck.GetDeckCount() == 0)
//            yield return RefillAndDraw(deck, drawIfHandEmpty);
//        else
//            yield return deck.StartCoroutine(deck.DrawCards(drawIfHandEmpty));
//    }

//    private IEnumerator RefillAndDraw(Deck deck, int count)
//    {
//        yield return new WaitForSeconds(0.5f);

//        List<CardInstance> discardCards = deck.discard.TakeAllCards();
//        deck.ReceiveCards(discardCards);
//        deck.ShuffleDeck();

//        yield return deck.StartCoroutine(deck.DrawCards(count));
//    }

//    public override int GetIntentValue() => damage;

//    public int GetIntentValue(int? overrideValue = null) => overrideValue ?? damage;
//}
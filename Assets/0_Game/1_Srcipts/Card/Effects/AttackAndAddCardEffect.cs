//using UnityEngine;

//[CreateAssetMenu(
//    fileName = "AttackAndAddCard",
//    menuName = "Cards/Effects/AttackAndAddCard"
//)]
//public class AttackAndAddCardEffect : EffectData, IOverrideValue
//{
//    [Header("Attack")]
//    public int damage = 7;
//    public CardType vfxType = CardType.Mellee;

//    [Header("Card Reward")]
//    public CardData cardToAdd;

//    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
//    {
//        if (target == null || cardToAdd == null || deck == null)
//            return false;

//        // 1. Gây damage
//        bool hit = self.DealDamage(target, damage, vfxType);

//        // 2. Nếu đánh trúng, thêm bài vào Draw Pile
//        if (hit)
//        {
//            deck.AddTemporaryCard(cardToAdd);
//            Debug.Log($"Added {cardToAdd.cardName} to Draw Pile");
//        }

//        return hit;
//    }

//    public override int GetIntentValue()
//    {
//        return damage;
//    }

//    public int GetIntentValue(int? overrideValue = null)
//    {
//        return overrideValue ?? damage;
//    }

//    public void ApplyWithOverride(
//        Character self,
//        Character target,
//        int overrideValue)
//    {
//        if (target == null || cardToAdd == null)
//            return;

//        bool hit = self.DealDamage(target, overrideValue, vfxType);

//        if (hit)
//        {
//            Deck.instance.AddCardToDrawPile(cardToAdd);
//        }
//    }
//}
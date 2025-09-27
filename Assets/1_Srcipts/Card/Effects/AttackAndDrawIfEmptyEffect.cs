using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackAndDrawIfEmpty", menuName = "Cards/Effects/AttackAndDrawIfEmpty")]
public class AttackAndDrawIfEmptyEffect : EffectData, IOverrideValue
{
    public int damage = 7;
    public CardType vfxType = CardType.Mellee;
    public int drawIfHandEmpty = 1;

    public override bool Apply(Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (target == null) return false;

        bool hit = self.DealDamage(target, damage, vfxType);

        if (hit)
        {
            // 👇 chạy coroutine để check sau khi lá bài bị remove khỏi hand
            self.StartCoroutine(DelayedCheck(deck));
        }

        return hit;
    }

    private IEnumerator DelayedCheck(Deck deck)
    {
        yield return null; // chờ 1 frame cho EnemyDropZone xóa card khỏi hand

        if (deck.GetCurrentHandObjects().Count == 0)
        {
            Debug.Log($"[AttackAndDrawIfEmptyEffect] Hand empty, draw {drawIfHandEmpty} cards");

            // Nếu deck + discard đều trống => không rút được gì
            if (deck.GetDeckCount() == 0 && deck.discard.Count == 0)
            {
                Debug.Log("[AttackAndDrawIfEmptyEffect] No cards in deck and discard!");
                yield break;
            }

            // Nếu deck rỗng nhưng discard còn => refill trước
            if (deck.GetDeckCount() == 0 && deck.discard.Count > 0)
            {
                yield return RefillAndDraw(deck, drawIfHandEmpty);
            }
            else
            {
                yield return deck.StartCoroutine(deck.DrawCards(drawIfHandEmpty));
            }
        }
    }

    private IEnumerator RefillAndDraw(Deck deck, int count)
    {
        Debug.Log("[AttackAndDrawIfEmptyEffect] Deck empty, refilling from discard...");

        // chờ hiệu ứng refill (nếu muốn có delay)
         yield return new WaitForSeconds(0.5f);

        // lấy toàn bộ discard đưa vào deck
        List<CardData> discardCards = deck.discard.TakeAllCards();
        deck.ReceiveCards(discardCards);
        deck.ShuffleDeck();

        // sau khi refill thì rút tiếp
        yield return deck.StartCoroutine(deck.DrawCards(count));
    }

    public override int GetIntentValue()
    {
        return damage;
    }

    public int GetIntentValue(int? overrideValue = null)
    {
        return overrideValue ?? damage;
    }

    public void ApplyWithOverride(Character self, Character target, int overrideValue)
    {
        if (target == null) return;

        int old = damage;
        damage = overrideValue;
        bool hit = self.DealDamage(target, damage);

        if (hit)
            Debug.Log($"[AttackEffect] {self.name} dealt {overrideValue} damage to {target.name} (override)");
        else
            Debug.Log($"[AttackEffect] {self.name} missed {target.name} (override)");

        damage = old;
    }
}

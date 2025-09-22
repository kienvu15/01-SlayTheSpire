using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    public Deck deck;
    public Discard discard;
    public ManaSystem manaSystem;
    public EnemySystem enemySystem;
    public Player player;
   // public Enemy enemy;

    public int handSize = 5;

    [Header("Animation/Timing")]
    public float waitAfterDiscardAnimations = 0.5f;
    public float waitBeforeRefill = 3f;   // thời gian chờ refill khi deck trống

    private bool isBusy = false;  
    public bool IsBusy => isBusy;

    private void Start()
    {
        if(deck == null)
        {
            deck = FindFirstObjectByType<Deck>();
        }
    }

    public void EndTurn()
    {
        if (isBusy)
        {
            Debug.Log("[Match] Đang bận (enemy/action/draw), không thể EndTurn!");
            return;
        }

        StartCoroutine(EndTurnRoutine());

        if (manaSystem != null)
        {
            manaSystem.StartTurn();
        }
    }

    private IEnumerator EndTurnRoutine()
    {
        isBusy = true;  // bắt đầu -> chặn

        // --- phần discard ---
        List<GameObject> handObjects = new List<GameObject>(deck.GetCurrentHandObjects());
        if (handObjects.Count > 0)
        {
            foreach (GameObject cardGO in handObjects)
            {
                if (cardGO == null) continue;
                CardDisplay cd = cardGO.GetComponent<CardDisplay>();
                if (cd != null) discard.AddToDiscard(cd.GetCardData());
            }

            yield return StartCoroutine(discard.AnimateDiscardHand(handObjects));
            deck.ClearHandReferencesOnly();
        }
        yield return new WaitForSeconds(waitAfterDiscardAnimations);

        // --- enemy turn ---
        if (enemySystem != null)
        {
            yield return StartCoroutine(enemySystem.EnemyTurn(player));
        }

        // --- draw hand ---
        yield return StartCoroutine(TryDrawNextHand());

        deck.UpdateUI();
        if (discard != null) discard.UpdateDiscardCount();

        isBusy = false; // xong xuôi -> mở khóa

        if (player != null)
            player.TickConditions();

        if (enemySystem != null)
        {
            foreach (var enemy in enemySystem.enemies) // giả sử enemySystem có list enemies
            {
                enemy.TickConditions();
            }
        }
    }


    private IEnumerator TryDrawNextHand()
    {
        int need = handSize;

        while (need > 0)
        {
            int available = deck.GetDeckCount();

            if (available > 0)
            {
                int drawNow = Mathf.Min(available, need);
                Debug.Log($"[Match] Drawing {drawNow} cards (need left {need})");
                yield return StartCoroutine(deck.DrawCards(drawNow));
                need -= drawNow;
            }
            else
            {
                // Deck hết -> chờ trước khi refill
                Debug.Log($"[Match] Deck empty. Waiting {waitBeforeRefill}s before refill...");
                yield return new WaitForSeconds(waitBeforeRefill);

                // Refill từ discard
                List<CardData> taken = discard.TakeAllCards();
                if (taken.Count > 0)
                {
                    Debug.Log($"[Match] Taken {taken.Count} cards from discard to deck.");
                    deck.ReceiveCards(taken);
                    deck.ShuffleDeck();
                    // tiếp vòng while sẽ rút tiếp
                }
                else
                {
                    Debug.Log("[Match] Discard also empty. Cannot draw more cards.");
                    break;
                }
            }
        }
    }

    public void PlayCard(CardData card, Character self, Character target, ManaSystem manaSystem, Deck deck)
    {
        if (card == null || card.effects == null) return;

        foreach (var wrapper in card.effects)
        {
            if (wrapper == null || wrapper.effect == null) continue;

            if (wrapper.overrideValue && wrapper.effect is IOverrideValue overridable)
            {
                overridable.ApplyWithOverride(self, target, wrapper.valueOverride);
            }
            else
            {
                wrapper.effect.Apply(self, target, manaSystem, deck);
            }
        }
    }


}

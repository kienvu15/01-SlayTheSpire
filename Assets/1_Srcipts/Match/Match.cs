using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : MonoBehaviour
{
    public Deck deck;
    public Discard discard;
    public ManaSystem manaSystem;
    public EnemySystem enemySystem;
    public PlayerSelfCast playerSelfCast;
    public Player player;

    public int handSize = 5;

    [Header("Animation/Timing")]
    public float waitAfterDiscardAnimations = 0.5f;
    public float waitBeforeRefill = 3f;   


    public void EndTurn()
    {
        if (GameStage.Instance.isBusy) return;
        if (player != null)
        {
            player.DecreaseEnemyConditions();
        }

        StartCoroutine(EndTurnRoutine());

        if (manaSystem != null)
            manaSystem.StartTurn();
    }


    private IEnumerator EndTurnRoutine()
    {
        GameStage.Instance.SetBusy(true);

        // --- discard ---
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

        // --- Enemy TURN ---
        if (enemySystem != null)
        {
            // StartTurn của enemy (trigger poison/regen/v.v…)
            foreach (var enemy in enemySystem.enemies)
                enemy.TriggerConditionEffects();

            yield return StartCoroutine(enemySystem.EnemyTurn(player));

            // EndTurn của enemy -> giảm condition do PLAYER cast lên enemy
            foreach (var enemy in enemySystem.enemies)
                enemy.DecreasePlayerConditions();

            // 🟢 giảm luôn condition mà Enemy tự cast lên Enemy
            foreach (var enemy in enemySystem.enemies)
                enemy.DecreaseEnemySelfConditions();

            // 🟢 giảm luôn condition mà Player tự cast lên Player
            player.DecreasePlayerSelfConditions();
        }

        // --- Player START TURN ---
        if (player != null)
        {
            player.TriggerConditionEffects();
        }

        // --- Draw hand ---
        yield return StartCoroutine(TryDrawNextHand());

        deck.UpdateUI();
        if (discard != null) discard.UpdateDiscardCount();

        GameStage.Instance.SetBusy(false);
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

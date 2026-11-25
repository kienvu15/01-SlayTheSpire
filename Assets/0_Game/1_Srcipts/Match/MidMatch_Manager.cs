using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidMatch_Manager : MonoBehaviour
{
    public static MidMatch_Manager instance;

    [Header("References")]
    public Deck deck;
    public Discard discard;
    public ManaSystem manaSystem;
    public PlayerSelfCast playerSelfCast;

    [Header("Ref Later in game")]
    public EnemySystem enemySystem;
    public Player player;

    [HideInInspector] public int handSize;

    [Header("Animation/Timing")]
    public float waitAfterDiscardAnimations = 0.5f;
    public float waitBeforeRefill = 3f;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            EndTurn();
        }
    }

    // Button END TURN được gọi từ UI
    public void EndTurn()
    {
        if (GameStage.Instance.isBusy) return;
        if (player != null)
        {
            player.DecreaseEnemyConditions();
        }

        StartCoroutine(EndTurnRoutine());
    }

    private IEnumerator EndTurnRoutine()
    {
        GameStage.Instance.SetBusy(true);

        // 1. Chờ mọi lá đã xếp hàng chạy xong
        yield return ActionQueue.Instance.WaitUntilIdle();

        // 2. Discard tay còn lại
        var handObjects = new List<GameObject>(deck.GetCurrentHandObjects());
        foreach (var go in handObjects)
        {
            var cd = go != null ? go.GetComponent<CardDisplay>() : null;
            if (cd != null && cd.cardInstance != null)
                discard.AddDirect(cd.cardInstance);
        }
        if (handObjects.Count > 0)
        {
            yield return StartCoroutine(discard.AnimateDiscardHand(handObjects));
            deck.ClearHandReferencesOnly();
        }
        yield return new WaitForSeconds(waitAfterDiscardAnimations);

        // 3. Enemy turn qua queue
        ActionQueue.Instance.Enqueue(new EnemyTurnAction());
        yield return ActionQueue.Instance.WaitUntilIdle();

        // 4. Player start turn
        if (player != null) player.TriggerConditionEffects();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(TryDrawNextHand());

        deck.UpdateUI();
        if (discard != null) discard.UpdateDiscardCount();
        GameStage.Instance.SetBusy(false);
    }

    private IEnumerator TryDrawNextHand()
    {
        if (manaSystem != null)
            manaSystem.StartTurn();

        Debug.Log($"[MidMatch_Manager] Trying to draw {handSize} cards for next hand.");
        int need = handSize;

        while (need > 0)
        {
            int available = deck.GetDeckCount();

            if (available > 0)
            {
                int drawNow = Mathf.Min(available, need);
                Debug.Log($"[MidMatch_Manager] Drawing {drawNow} cards (need left {need})");
                yield return StartCoroutine(deck.DrawCards(drawNow));
                need -= drawNow;
            }
            else
            {
                // Deck hết -> chờ trước khi refill
                Debug.Log($"[MidMatch_Manager] Deck empty. Waiting {waitBeforeRefill}s before refill...");
                yield return new WaitForSeconds(waitBeforeRefill);

                // Refill từ discard
                List<CardInstance> taken = discard.TakeAllCards();
                if (taken.Count > 0)
                {
                    Debug.Log($"[MidMatch_Manager] Taken {taken.Count} cards from discard to deck.");
                    deck.ReceiveCards(taken);
                    deck.ShuffleDeck();
                }
                else
                {
                    Debug.Log("[MidMatch_Manager] Discard also empty. Cannot draw more cards.");
                    break;
                }
            }
        }
    }

}

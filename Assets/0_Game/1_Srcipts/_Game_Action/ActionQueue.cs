using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    public static ActionQueue Instance;

    [Header("References")]
    public Deck deck;
    public Discard discard;
    public ManaSystem mana;
    public MidMatch_Manager match;

    [Header("Pacing")]
    public float gapBetweenEffects = 0.15f;
    public float gapBetweenActions = 0.1f;

    private readonly Queue<GameAction> queue = new();
    private Coroutine runner;

    public bool IsPlayerInputLocked => IsProcessing;
    public bool IsProcessing { get; private set; }
    public int Pending => queue.Count + (IsProcessing ? 1 : 0);

    private void Awake() => Instance = this;

    public void Enqueue(GameAction action)
    {
        queue.Enqueue(action);
        if (runner == null) runner = StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        IsProcessing = true;
        while (queue.Count > 0)
        {
            GameAction action = queue.Dequeue();
            // Lồng coroutine: nếu 1 action ném exception, chỉ action đó chết, queue vẫn chạy tiếp
            yield return StartCoroutine(action.Execute(this));
            if (gapBetweenActions > 0) yield return new WaitForSeconds(gapBetweenActions);
        }
        IsProcessing = false;
        runner = null;
    }

    // EndTurn, enemy turn... dùng để chờ queue chạy xong
    public IEnumerator WaitUntilIdle()
    {
        while (Pending > 0) yield return null;
    }

    // ===== Cửa vào duy nhất cho việc đánh bài =====
    public bool TryEnqueuePlay(CardDisplay display, DraggableCard draggable, Character target)
    {
        if (GameStage.Instance.isBusy) return false;

        CardInstance card = display.cardInstance;
        if (card == null || card.data.isUnplayable) return false;
        if (!mana.CanPlayCard(card)) return false;
        // Giữ mana ngay để người chơi không tiêu quá số mana khi kéo nhanh
        mana.SpendMana(card.ManaCost);
        // Rời tay NGAY: layout, Requirement ("còn lại trên tay"), IfHandEmpty đều thấy tay đúng
        deck.RemoveCardFromHand(display.gameObject, match);
        draggable.MarkAsValidDrop();
        RectTransform rect = display.GetComponent<RectTransform>();
        rect.SetParent(discard.GetComponent<RectTransform>().parent, true);
        StartCoroutine(LockNextFrame(display));
        Enqueue(new PlayCardAction(display, target, card.data.IsOpponentCast()));
        return true;
    }

    // Chờ OnEndDrag chạy xong rồi mới khóa (component bị disable sẽ không nhận OnEndDrag)
    private IEnumerator LockNextFrame(CardDisplay display)
    {
        yield return null;
        if (display == null) yield break;
        var cg = display.GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = false;
        var drag = display.GetComponent<DraggableCard>();
        if (drag != null) drag.enabled = false;
        if (display.card_Button != null) display.card_Button.interactable = false;
    }

}
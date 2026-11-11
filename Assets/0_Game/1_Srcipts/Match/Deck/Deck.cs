using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DrawAnimationType { Straight, Arc }
public enum DrawPileInsert { Top, Bottom, Random }

public class Deck : MonoBehaviour
{
    public static Deck instance;

    [Header("References")]
    public CardPileViewer pileViewer;
    public Discard discard;
    public GameObject cardPrefab;
    public Transform handParent;
    public Transform pileDeckTransform;

    public MidMatch_Manager match;

    [Header("UI Debug")]
    public bool hasDrawFisrtRow = false;
    public TextMeshProUGUI[] deckCountText;
    public TextMeshProUGUI discardCountText;
    public TextMeshProUGUI allCardCount;

    [Header("Animation Settings")]
    public float drawDuration = 0.6f;
    public float drawDelay = 0.15f;
    public float refillDelay = 3f;

    [Header("Draw Fly Animation")]
    public int spinTurns = 1;
    public float arcHeightRatio = 0.25f;
    public float startScale = 0.5f;
    public float peakScale = 1.2f;

    // Draw pile / toàn bộ bài / bài tạm: đều là CardInstance (giữ level)
    private List<CardInstance> deck = new List<CardInstance>();
    private List<CardInstance> allCards = new List<CardInstance>();
    private List<GameObject> currentHand = new List<GameObject>();
    private List<CardInstance> temporaryCards = new List<CardInstance>();
    private List<CardInstance> temporarilyRemovedCards = new List<CardInstance>();

    [Header("NewSystem")]
    public HandLayout handLayout;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateUI();
        UpdateAllCardCount();
    }

    // Trả về cùng instance (không clone) để màn nâng cấp sửa trực tiếp được
    public List<CardInstance> GetAllCards()
    {
        return new List<CardInstance>(allCards);
    }

    public void RemoveCardFromHand(GameObject cardGO, MidMatch_Manager matchRef)
    {
        if (cardGO == null) return;

        if (currentHand.Contains(cardGO))
            currentHand.Remove(cardGO);

        handLayout.ArrangeCards(currentHand, true);
        UpdateUI();
    }

    #region Build
    // ================= BUILD =================

    // Dùng cái này: bộ bài lưu xuyên suốt run (giữ level đã nâng cấp)
    public void BuildDeck(List<CardInstance> startingCards)
    {
        deck.Clear();
        allCards.Clear();

        if (startingCards == null || startingCards.Count == 0)
        {
            UpdateUI();
            return;
        }

        deck.AddRange(startingCards);
        allCards.AddRange(startingCards);
        Shuffle(deck);
        UpdateUI();
        UpdateAllCardCount();
    }

    // Overload cũ: bài mới hoàn toàn, level 1
    public void BuildDeck(List<CardData> startingCards)
    {
        if (startingCards == null)
        {
            BuildDeck((List<CardInstance>)null);
            return;
        }

        var instances = new List<CardInstance>();
        foreach (var data in startingCards)
            if (data != null) instances.Add(new CardInstance(data));

        BuildDeck(instances);
    }
    #endregion
    #region Draw
    // ================= DRAW =================
    public IEnumerator DrawHand(int count)
    {
        yield return StartCoroutine(DrawCards(count));
    }

    public IEnumerator DrawCards(int count)
    {
        if (handParent == null || handLayout == null) yield break;

        int drawCount = Mathf.Min(count, deck.Count);
        for (int i = 0; i < drawCount; i++)
        {
            yield return StartCoroutine(DrawOneCard(currentHand.Count));
            UpdateUI();
        }
    }

    private IEnumerator DrawOneCard(int handIndex)
    {
        yield return DrawOneCard(handIndex, DrawAnimationType.Straight);
    }

    private IEnumerator DrawOneCard(int handIndex, DrawAnimationType animationType)
    {
        if (deck.Count == 0 || handParent == null ||
            pileDeckTransform == null || handLayout == null)
        {
            Debug.LogError("[DrawOneCard] Deck empty");
            yield break;
        }

        CardInstance drawnCard = deck[0];
        deck.RemoveAt(0);

        GameObject cardGO = Instantiate(cardPrefab, handParent, false);
        currentHand.Add(cardGO);

        CardDisplay display = cardGO.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.LoadCard(drawnCard);   // truyền INSTANCE để giữ level
            display.card_Button.onClick.AddListener(() => display.OnCardClick());
        }

        RectTransform rect = cardGO.GetComponent<RectTransform>();
        if (rect == null)
        {
            currentHand.Remove(cardGO);
            Destroy(cardGO);
            yield break;
        }

        DraggableCard draggable = cardGO.GetComponent<DraggableCard>();
        if (draggable != null)
            draggable.enabled = false;

        int cardIndex = currentHand.IndexOf(cardGO);

        if (!handLayout.TryGetCardTarget(currentHand, cardIndex,
            out Vector2 targetPosition, out Quaternion targetRotation))
        {
            currentHand.Remove(cardGO);
            Destroy(cardGO);
            yield break;
        }

        handLayout.ArrangeCards(currentHand, true);

        Vector3 startPosition = pileDeckTransform.position;
        Vector3 targetWorld = handParent.TransformPoint(targetPosition);

        float targetZ = targetRotation.eulerAngles.z;
        if (targetZ > 180f) targetZ -= 360f;

        rect.DOKill();
        rect.position = startPosition;
        rect.localScale = Vector3.zero;
        rect.localRotation = Quaternion.identity;

        float duration = Mathf.Max(0.1f, drawDuration);

        Sequence sequence = DOTween.Sequence().SetLink(cardGO);

        #region animations
        if (animationType == DrawAnimationType.Straight)
        {
            sequence.Append(
                rect.DOMove(targetWorld, duration).SetEase(Ease.InOutQuad)
            );
        }
        else
        {
            Vector3 midpoint = Vector3.Lerp(startPosition, targetWorld, 0.5f);
            midpoint.y += Vector3.Distance(startPosition, targetWorld) * arcHeightRatio;

            sequence.Append(
                rect.DOPath(
                    new[] { startPosition, midpoint, targetWorld },
                    duration,
                    PathType.CatmullRom,
                    PathMode.Full3D
                ).SetEase(Ease.OutCubic)
            );
        }

        sequence.Join(
            rect.DOLocalRotate(
                new Vector3(0f, 0f, targetZ + 360f * spinTurns),
                duration,
                RotateMode.FastBeyond360
            ).SetEase(Ease.OutQuad)
        );

        sequence.Join(
            rect.DOScale(Vector3.one, duration).SetEase(Ease.OutBack)
        );

        yield return sequence.WaitForCompletion();
        #endregion

        if (cardGO == null) yield break;
        handLayout.ArrangeCards(currentHand, false);
        if (draggable != null)
            draggable.enabled = true;
    }
    #endregion

    // Thêm vào draw pile tại vị trí chỉ định
    public void AddCardToDrawPile(CardInstance card, DrawPileInsert insert, bool temporary)
    {
        if (card == null || IsTemporarilyRemoved(card)) return;

        switch (insert)
        {
            case DrawPileInsert.Top: deck.Insert(0, card); break;
            case DrawPileInsert.Random: deck.Insert(Random.Range(0, deck.Count + 1), card); break;
            default: deck.Add(card); break;
        }

        allCards.Add(card);
        if (temporary) temporaryCards.Add(card);
        UpdateUI();
    }

    // Thêm vào discard pile
    public void AddCardToDiscardPile(CardInstance card, bool temporary)
    {
        if (card == null || discard == null || IsTemporarilyRemoved(card)) return;

        allCards.Add(card);
        if (temporary) temporaryCards.Add(card);
        discard.AddToDiscard(card); //  ?????
        UpdateUI();
    }

    // ================= TEMPORARY CARDS =================
    public void ClearTemporaryCards()
    {
        foreach (CardInstance card in temporaryCards)
        {
            if (card == null) continue;
            deck.Remove(card);
            allCards.Remove(card);
            if (discard != null) discard.discardPile.Remove(card);
        }

        temporaryCards.Clear();
        if (discard != null) discard.UpdateDiscardCount();
        UpdateUI();
        UpdateAllCardCount();
    }

    public bool IsTemporaryCard(CardInstance card)
    {
        return temporaryCards.Contains(card);
    }

    // Gọi khi kết thúc trận
    public void ResetTemporaryUpgrades()
    {
        foreach (var card in allCards)
            if (card != null) card.ClearTempUpgrade();
    }

    public bool RemoveTemporaryCard(CardInstance card)
    {
        if (card == null || temporarilyRemovedCards.Contains(card))
            return false;

        temporarilyRemovedCards.Add(card);

        // Xóa khỏi Draw Pile
        deck.Remove(card);

        // Xóa khỏi Discard Pile
        if (discard != null)
        {
            discard.discardPile.Remove(card);
            discard.UpdateDiscardCount();
        }

        // Xóa khỏi Hand nếu đang có trên tay
        GameObject handCard = null;

        foreach (var go in currentHand)
        {
            if (go == null) continue;

            CardDisplay display = go.GetComponent<CardDisplay>();

            if (display != null && display.cardInstance == card)
            {
                handCard = go;
                break;
            }
        }

        if (handCard != null)
        {
            currentHand.Remove(handCard);
            Destroy(handCard);

            if (handLayout != null)
                handLayout.ArrangeCards(currentHand, true);
        }

        UpdateUI();

        return true;
    }

    // Overload 2: Loại ngẫu nhiên một lá bài trên tay
    public CardInstance RemoveTemporaryCard()
    {
        List<CardInstance> candidates = GetHandCards();

        // Không chọn những lá đã bị loại
        candidates.RemoveAll(card =>
            card == null || temporarilyRemovedCards.Contains(card));

        if (candidates.Count == 0)
            return null;

        CardInstance selected = candidates[Random.Range(0, candidates.Count)];

        RemoveTemporaryCard(selected);

        return selected;
    }

    // Kiểm tra lá bài đã bị loại tạm thời chưa
    public bool IsTemporarilyRemoved(CardInstance card)
    {
        return card != null && temporarilyRemovedCards.Contains(card);
    }

    // Reset khi bắt đầu trận mới
    public void ResetTemporarilyRemovedCards()
    {
        temporarilyRemovedCards.Clear();
    }

    // ================= Upgrade CARDS =================

    // Nâng cấp ngẫu nhiên n lá trên tay (bỏ qua lá đang đánh). Trả về số lá đã nâng cấp.
    public int UpgradeRandomInHand(int count, CardInstance exclude = null)
    {
        var candidates = new List<CardDisplay>();

        foreach (var go in currentHand)
        {
            if (go == null) continue;
            CardDisplay d = go.GetComponent<CardDisplay>();
            if (d == null || d.cardInstance == null) continue;
            if (d.cardInstance == exclude) continue;
            if (!d.cardInstance.CanUpgrade) continue;
            candidates.Add(d);
        }

        Shuffle(candidates);
        int n = Mathf.Min(count, candidates.Count);

        for (int i = 0; i < n; i++)
        {
            if (candidates[i].cardInstance.UpgradeTemp())
                candidates[i].Refresh();   // cập nhật tên, mana, mô tả, artwork
        }
        return n;
    }

    // Nâng cấp ngẫu nhiên n lá trong draw pile
    public int UpgradeRandomInDrawPile(int count)
    {
        var candidates = deck.FindAll(c => c != null && c.CanUpgrade);

        Shuffle(candidates);
        int n = Mathf.Min(count, candidates.Count);

        for (int i = 0; i < n; i++)
            candidates[i].UpgradeTemp();

        UpdateUI();
        return n;
    }

    // ================= Helper =================
    public List<CardInstance> GetRemainingHandInstances(CardInstance excludeCard)
    {
        List<CardInstance> result = new List<CardInstance>();
        foreach (var cardGO in GetCurrentHandObjects())
        {
            if (cardGO == null) continue;
            CardDisplay display = cardGO.GetComponent<CardDisplay>();
            if (display == null || display.cardInstance == null) continue;
            if (display.cardInstance == excludeCard) continue;
            result.Add(display.cardInstance);
        }
        return result;
    }

    // Lấy danh sách bài trong Draw Pile
    public List<CardInstance> GetDrawPileCards()
    {
        return new List<CardInstance>(deck);
    }

    // Lấy danh sách bài trong Discard Pile
    public List<CardInstance> GetDiscardPileCards()
    {
        if (discard == null)
            return new List<CardInstance>();

        return new List<CardInstance>(discard.discardPile);
    }

    public void MoveDiscardToHand(List<CardInstance> cards)
    {
        if (cards == null || cards.Count == 0 || discard == null) return;

        int moved = 0;
        foreach (var card in cards)
        {
            if (card == null) continue;
            if (!discard.discardPile.Remove(card)) continue;   // không còn trong discard thì bỏ qua
            deck.Insert(0, card);                              // lên đỉnh draw pile để rút ngay
            moved++;
        }

        discard.UpdateDiscardCount();
        UpdateUI();

        //if (moved > 0)
          //  StartCoroutine(DrawCards(moved));
    }

    public List<CardInstance> GetHandCards(CardInstance exclude = null)
    {
        var list = new List<CardInstance>();
        foreach (var go in currentHand)
        {
            if (go == null) continue;
            var d = go.GetComponent<CardDisplay>();
            if (d == null || d.cardInstance == null || d.cardInstance == exclude) continue;
            list.Add(d.cardInstance);
        }
        return list;
    }

    //==================================================
    // Overload cũ
    public bool IsTemporaryCard(CardData data)
    {
        return temporaryCards.Exists(c => c.data == data);
    }

    public void AddTemporaryCard(CardInstance card)
    {
        if (card == null) return;

        deck.Add(card);
        allCards.Add(card);
        temporaryCards.Add(card);

        UpdateUI();
    }

    public void AddTemporaryCard(CardData cardData)
    {
        if (cardData == null) return;
        AddTemporaryCard(new CardInstance(cardData));
    }

    // ================= ADD / REMOVE =================
    public void AddCardToDrawPile(CardInstance card)
    {
        if (card == null) return;
        deck.Add(card);
        allCards.Add(card);
        UpdateUI();
        UpdateAllCardCount();
    }

    public void AddCardToDrawPile(CardData cardData)
    {
        if (cardData == null) return;
        AddCardToDrawPile(new CardInstance(cardData));
    }

    public void AddCard(CardInstance card)
    {
        if (card == null || IsTemporarilyRemoved(card)) return;
        deck.Add(card);
        UpdateUI();
    }

    public void AddCard(CardData cardData)
    {
        if (cardData == null) return;
        AddCard(new CardInstance(cardData));
    }

    // Nhận bài vào đáy deck (đang dùng cho xáo discard về?)
    public void ReceiveCards(List<CardInstance> cards)
    {
        if (cards == null || cards.Count == 0) return;
        deck.AddRange(cards);
        UpdateUI();
    }

    public void RemoveFromDeck(CardInstance card)
    {
        if (card != null) deck.Remove(card);
        UpdateUI();
    }

    // Overload cũ: xóa lá đầu tiên có data tương ứng
    public void RemoveFromDeck(CardData data)
    {
        int i = deck.FindIndex(c => c.data == data);
        if (i >= 0) deck.RemoveAt(i);
        UpdateUI();
    }

    public void RemoveCardFromCollection(CardInstance card)
    {
        if (card == null) return;
        allCards.Remove(card);
        deck.Remove(card);
        UpdateUI();
    }

    // Overload cũ: xóa MỘT lá có data tương ứng (không xóa hết bản sao)
    public void RemoveCardFromCollection(CardData data)
    {
        if (data == null) return;

        int i = allCards.FindIndex(c => c.data == data);
        if (i < 0) return;

        RemoveCardFromCollection(allCards[i]);
    }

    // ================= HAND =================
    private int GetFirstEmptySlot()
    {
        for (int i = 0; i < handParent.childCount; i++)
        {
            if (handParent.GetChild(i).childCount == 0)
                return i;
        }
        return -1;
    }

    public void DestroyAndClearHand()
    {
        foreach (GameObject card in currentHand)
        {
            if (card != null) Destroy(card);
        }
        currentHand.Clear();
    }

    public IEnumerator AnimateClearHand()
    {
        currentHand.RemoveAll(card => card == null);
        yield return StartCoroutine(discard.AnimateDiscardHand(currentHand));
        currentHand.Clear();
        UpdateUI();
    }

    public void ClearHandReferencesOnly()
    {
        currentHand.Clear();
    }

    public List<GameObject> GetCurrentHandObjects()
    {
        currentHand.RemoveAll(card => card == null);
        return new List<GameObject>(currentHand);
    }

    public int GetDeckCount()
    {
        return deck.Count;
    }

    public void ShuffleDeck()
    {
        Shuffle(deck);
        UpdateUI();
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    // ================= UI =================
    public void UpdateAllCardCount() { }

    public void UpdateUI()
    {
        if (deckCountText != null)
        {
            foreach (var text in deckCountText)
            {
                if (text != null)
                    text.text = "Draw Pile - " + deck.Count;
            }
        }

        if (discardCountText != null && discard != null)
            discardCountText.text = "Discard Pile - " + discard.Count;
    }
}